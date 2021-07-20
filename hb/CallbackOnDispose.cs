using System;
using System.Threading;

namespace hb
{
    /// <summary>
    /// 释放时触发回调
    /// </summary>
    public sealed class CallbackOnDispose : IDisposable
    {
        private Action _action;

        public CallbackOnDispose(Action action)
        {
            this._action = action ?? throw new ArgumentNullException("action");
        }
        public void Dispose()
        {
            Interlocked.Exchange(ref _action, null)?.Invoke();
        }
    }
}
