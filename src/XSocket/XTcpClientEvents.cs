using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XSocket
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/29 0:58:48
    /// description:
    /// </summary>
    public class XTcpClientEvents
    {
        public event EventHandler<ConnectionEventArgs> ServerConnected;

        public event EventHandler<ExceptionEventArgs> ExceptionEncountered;

        internal void HandleServerConnected(object sender, ConnectionEventArgs args)
        {
            WrappedEventHandler(() => ServerConnected?.Invoke(sender, args), "ServerConnected", sender);
        }

        internal void HandleExceptionEncountered(object sender, ExceptionEventArgs args)
        {
            WrappedEventHandler(() => ExceptionEncountered?.Invoke(sender, args), "ExceptionEncountered", sender);
        }

        internal void WrappedEventHandler(Action action, string handler, object sender)
        {
            if (action == null) return;

            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Action<Severity, string> logger = ((XTcpClient)sender).Settings?.Logger;
                logger?.Invoke(Severity.Error, "Event handler exception in " + handler + ": "
                    + Environment.NewLine + ex.Message
                    + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
