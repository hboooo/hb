using hb.LogServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 19:32:46
    /// description:文件监控
    /// </summary>
    public class FileWatcher : IDisposable
    {
        private int _interval = 10 * 1000;
        private bool _isWatching = false;
        private ConcurrentDictionary<string, FileSystemWatcher> _watchers = new ConcurrentDictionary<string, FileSystemWatcher>();


        /// <summary>
        /// 文件更改监控已启动
        /// </summary>
        public bool IsWatching
        {
            get { return _isWatching; }
        }

        public FileWatcher()
        {
        }

        public FileWatcher(string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                throw new ArgumentException("path must be not null");
            }
            foreach (var path in paths)
            {
                AddPath(path);
            }
        }


        public bool AddPath(string path)
        {
            if (Directory.Exists(path))
            {
                if (!_watchers.ContainsKey(path))
                {
                    _watchers[path] = null;
                    return true;
                }
            }
            return false;
        }


        public bool RemovePath(string path)
        {
            FileSystemWatcher fileSystemWatcher;
            bool ret = _watchers.TryRemove(path, out fileSystemWatcher);
            if (ret && fileSystemWatcher != null)
            {
                StopWatcher(fileSystemWatcher);
            }
            return ret;
        }


        public void Start()
        {
            if (_isWatching)
            {
                return;
            }
            _isWatching = true;
            Task.Factory.StartNew(() =>
            {
                while (_isWatching)
                {
                    HashSet<string> keys = new HashSet<string>();
                    foreach (var item in _watchers)
                    {
                        if (!Directory.Exists(item.Key))
                        {
                            StopWatcher(item.Value);
                            keys.Add(item.Key);
                            continue;
                        }

                        if (item.Value == null)
                        {
                            _watchers.TryUpdate(item.Key, CreateWatcher(item.Key), null);
                        }
                    }
                    foreach (string key in keys)
                    {
                        RemovePath(key);
                    }
                    Thread.Sleep(_interval);
                }
            });
        }


        public void Stop()
        {
            _isWatching = false;
            foreach (var watcher in _watchers)
            {
                StopWatcher(watcher.Value);
            }
        }


        private void StopWatcher(FileSystemWatcher fileSystemWatcher)
        {
            try
            {
                fileSystemWatcher.EnableRaisingEvents = false;
                fileSystemWatcher.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        private FileSystemWatcher CreateWatcher(string path)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(path);
            fsw.Created += Fsw_Changed;
            fsw.Changed += Fsw_Changed;
            fsw.Deleted += Fsw_Changed;
            fsw.Renamed += Fsw_Renamed;
            fsw.IncludeSubdirectories = true;
            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Attributes | NotifyFilters.Size;
            fsw.EnableRaisingEvents = true;
            return fsw;
        }


        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            _watcherEvent?.Invoke(sender, new FileWatcherEventArgs(e.ChangeType, e.FullPath, Path.GetFileName(e.FullPath), null, null));
        }


        private void Fsw_Renamed(object sender, RenamedEventArgs e)
        {
            _watcherEvent?.Invoke(sender, new FileWatcherEventArgs(e.ChangeType, e.FullPath, Path.GetFileName(e.FullPath), e.OldFullPath, e.OldName));
        }

        public void Dispose()
        {
            try
            {
                Stop();
                _watchers.Clear();
                WatcherEvent -= _watcherEvent;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private event FileWatcherEventHandler _watcherEvent;
        public event FileWatcherEventHandler WatcherEvent
        {
            add
            {
                _watcherEvent += value;
            }
            remove
            {
                _watcherEvent -= value;
            }
        }

        public delegate void FileWatcherEventHandler(object sender, FileWatcherEventArgs args);
    }


    public class FileWatcherEventArgs : EventArgs
    {
        public FileWatcherEventArgs(WatcherChangeTypes type, string fullPath, string name, string oldFullPath, string oldName)
        {
            changeType = type;
            this.fullPath = fullPath;
            this.name = name;
            this.oldFullPath = oldFullPath;
            this.oldName = oldName;
        }

        private WatcherChangeTypes changeType;

        public WatcherChangeTypes MyProperty
        {
            get { return changeType; }
            set { changeType = value; }
        }


        private string fullPath;

        public string FullPath
        {
            get { return fullPath; }
            set { fullPath = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string oldFullPath;

        public string OldFullPath
        {
            get { return oldFullPath; }
            set { oldFullPath = value; }
        }

        private string oldName;

        public string OldName
        {
            get { return oldName; }
            set { oldName = value; }
        }

    }
}
