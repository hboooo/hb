using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XSocket
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/29 0:57:16
    /// description:
    /// </summary>
    public class XTcpClient : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public XTcpClientSettings Settings
        {
            get
            {
                return _Settings;
            }
            set
            {
                if (value == null) _Settings = new XTcpClientSettings();
                else _Settings = value;
            }
        }

        public XTcpClientEvents Events
        {
            get
            {
                return _Events;
            }
            set
            {
                if (value == null) _Events = new XTcpClientEvents();
                else _Events = value;
            }
        }

        public XTcpClientCallbacks Callbacks
        {
            get
            {
                return _Callbacks;
            }
            set
            {
                if (value == null) _Callbacks = new XTcpClientCallbacks();
                else _Callbacks = value;
            }
        }

        public XTcpStatistics Statistics
        {
            get
            {
                return _Statistics;
            }
        }

        public XTcpKeepaliveSettings Keepalive
        {
            get
            {
                return _Keepalive;
            }
            set
            {
                if (value == null) _Keepalive = new XTcpKeepaliveSettings();
                else _Keepalive = value;
            }
        }

        public bool Connected { get; private set; }


        private XTcpClientSettings _Settings = new XTcpClientSettings();
        private XTcpClientEvents _Events = new XTcpClientEvents();
        private XTcpClientCallbacks _Callbacks = new XTcpClientCallbacks();
        private XTcpStatistics _Statistics = new XTcpStatistics();
        private XTcpKeepaliveSettings _Keepalive = new XTcpKeepaliveSettings();

        private string _ServerIp = null;
        private int _ServerPort = 0;

        private TcpClient _Client;
        private Stream _DataStream = null;
        private NetworkStream _TcpStream = null;

        private SemaphoreSlim _WriteLock = new SemaphoreSlim(1, 1);
        private SemaphoreSlim _ReadLock = new SemaphoreSlim(1, 1);

        private CancellationTokenSource _TokenSource = new CancellationTokenSource();
        private CancellationToken _Token;
        private Task _DataReceiver = null;
        private Task _MonitorSyncResponses = null;

        private readonly object _SyncResponseLock = new object();
        private Dictionary<string, XResponse> _SyncResponses = new Dictionary<string, XResponse>();

        public XTcpClient(string serverIp, int serverPort)
        {
            _ServerIp = serverIp ?? throw new ArgumentNullException(nameof(serverIp));
            if (serverPort < 0 || serverPort > 65535) throw new ArgumentOutOfRangeException(nameof(serverPort));
            _ServerPort = serverPort;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Connect()
        {
            if (Connected)
            {
                _Settings.Logger?.Invoke(Severity.Error, $"Already connected to the server. {_ServerIp}:{_ServerPort}");
                throw new InvalidOperationException($"Already connected to the server. {_ServerIp}:{_ServerPort}");
            }

            _Client = new TcpClient();
            _Statistics = new XTcpStatistics();

            _Settings.Logger?.Invoke(Severity.Info, $"Connecting to {_ServerIp}:{_ServerPort}");
            _Client.LingerState = new LingerOption(true, 0);

            bool connectSuccess = false;
            IAsyncResult asyncResult = _Client.BeginConnect(_ServerIp, _ServerPort, null, null);
            WaitHandle waitHandle = asyncResult.AsyncWaitHandle;

            try
            {
                connectSuccess = waitHandle.WaitOne(TimeSpan.FromSeconds(_Settings.ConnectTimeoutSeconds), false);
                if (!connectSuccess)
                {
                    _Client.Close();
                    _Settings.Logger?.Invoke(Severity.Error, $"Timeout connecting to {_ServerIp}:{_ServerPort}");
                    throw new TimeoutException($"Timeout connecting to {_ServerIp}:{_ServerPort}");
                }

                _Client.EndConnect(asyncResult);
                _TcpStream = _Client.GetStream();
                _DataStream = _TcpStream;

                if (_Keepalive.EnableTcpKeepAlives)
                {
                    EnableKeepalives();
                }

                Connected = true;
            }
            catch (Exception ex)
            {
                _Settings.Logger?.Invoke(Severity.Error, "Exception encountered: "
                    + Environment.NewLine + ex.Message
                    + Environment.NewLine + ex.StackTrace);
                _Events.HandleExceptionEncountered(this, new ExceptionEventArgs(ex));
            }
            finally
            {
                waitHandle.Close();
            }

            _TokenSource = new CancellationTokenSource();
            _Token = _TokenSource.Token;

            //_DataReceiver = Task.Factory.StartNew(DataReceiver, _Token);
            //_MonitorSyncResponses = Task.Factory.StartNew(MonitorForExpiredSyncResponses, _Token);
            _Events.HandleServerConnected(this, new ConnectionEventArgs($"{_ServerIp}:{_ServerPort}"));
            _Settings.Logger?.Invoke(Severity.Info, $"Connected to {_ServerIp}:{_ServerPort}");

        }

        public void Disconnect()
        {
            if (!Connected)
            {
                _Settings.Logger?.Invoke(Severity.Error, $"Not connected to the server. {_ServerIp}:{_ServerPort}");
                throw new InvalidOperationException($"Not connected to the server. {_ServerIp}:{_ServerPort}");
            }

            _Settings.Logger?.Invoke(Severity.Info, $"Disconnecting from {_ServerIp}:{_ServerPort}");

            if (_TokenSource != null)
            {
                if (!_TokenSource.IsCancellationRequested)
                {
                    _TokenSource.Cancel();
                    _TokenSource.Dispose();
                }
            }
            _TcpStream?.Close();
            _Client?.Close();

            while (_DataReceiver?.Status == TaskStatus.Running)
            {
                _DataReceiver.Wait(1);
            }
            Connected = false;
            _Settings.Logger?.Invoke(Severity.Info, $"Disconnected from {_ServerIp}:{_ServerPort}");

        }

        private void EnableKeepalives()
        {
            try
            {
                byte[] keepAlive = new byte[12];
                Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, keepAlive, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes((uint)(_Keepalive.TcpKeepAliveTime * 1000)), 0, keepAlive, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes((uint)(_Keepalive.TcpKeepAliveInterval * 1000)), 0, keepAlive, 8, 4);
                _Client.Client.IOControl(IOControlCode.KeepAliveValues, keepAlive, null);
            }
            catch (Exception)
            {
                _Settings.Logger?.Invoke(Severity.Error, "Keepalives not supported on this platform, disabled");
                _Keepalive.EnableTcpKeepAlives = false;
            }
        }

        public bool Send(string data)
        {
            if (string.IsNullOrEmpty(data)) return Send(new byte[0]);
            else return Send(Encoding.UTF8.GetBytes(data));
        }

        public bool Send(byte[] data, int start = 0)
        {
            if (data == null) data = new byte[0];
            Utils.BytesToStream(data, start, out int contentLength, out Stream stream);
            return Send(contentLength, stream);
        }

        public bool Send(long contentLength, Stream stream)
        {
            if (contentLength < 0) throw new ArgumentException("Content length must be zero or greater.");
            if (stream == null) stream = new MemoryStream(new byte[0]);
            XMessage msg = new XMessage(contentLength, stream);
            return SendInternal(msg, contentLength, stream);
        }


        private bool SendInternal(XMessage msg, long contentLength, Stream stream)
        {
            if (msg == null) throw new ArgumentNullException(nameof(msg));
            if (!Connected) return false;

            if (contentLength > 0 && (stream == null || !stream.CanRead))
            {
                throw new ArgumentException("Cannot read from supplied stream.");
            }

            bool disconnectDetected = false;

            if (_Client == null || !_Client.Connected)
            {
                disconnectDetected = true;
                return false;
            }

            _WriteLock.Wait();

            try
            {
                SendDataStream(contentLength, stream);

                _Statistics.IncrementSentMessages();
                _Statistics.AddSentBytes(contentLength);
                return true;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _Settings.Logger?.Invoke(Severity.Error,
                    $"Failed to write message to {_ServerIp}:{_ServerPort}"
                    + Environment.NewLine + ex.Message
                    + Environment.NewLine + ex.StackTrace);

                _Events.HandleExceptionEncountered(this, new ExceptionEventArgs(ex));

                disconnectDetected = true;
                return false;
            }
            finally
            {
                _WriteLock.Release();

                if (disconnectDetected)
                {
                    Connected = false;
                    Dispose();
                }
            }
        }

        private void SendDataStream(long contentLength, Stream stream)
        {
            if (contentLength <= 0) return;

            long bytesRemaining = contentLength;
            int bytesRead = 0;
            byte[] buffer = new byte[_Settings.StreamBufferSize];

            while (bytesRemaining > 0)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    _DataStream.Write(buffer, 0, bytesRead);
                    bytesRemaining -= bytesRead;
                }
            }

            _DataStream.Flush();
        }


        protected virtual void Dispose(bool disposing)
        {

        }

    }
}
