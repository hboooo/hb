using System;
using System.Threading;

namespace hb
{
    /// <summary>
    /// 释放时触发回调
    /// </summary>
    public sealed class CallbackOnDispose : IDisposable
    {
        private Action _callback;

        public CallbackOnDispose(Action callback)
        {
            this._callback = callback ?? throw new ArgumentNullException("callback");
        }
        public void Dispose()
        {
            Interlocked.Exchange(ref _callback, null)?.Invoke();
        }
    }
}
