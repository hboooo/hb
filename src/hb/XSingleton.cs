using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/8/1 0:16:57
    /// description:
    /// </summary>
    public abstract class XSingleton : IDisposable
    {
        ~XSingleton()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool isDisposing);

        private static readonly List<XSingleton> Singletons = new List<XSingleton>();

        protected XSingleton()
        {
            lock (Singletons)
            {
                Singletons.Add(this);
            }
        }

        public static void ClearAllSingletons()
        {
            lock (Singletons)
            {
                foreach (var s in Singletons)
                {
                    s.Dispose();
                }
                Singletons.Clear();
            }
        }

    }

    public abstract class XSingleton<T> : XSingleton where T : class
    {
        protected XSingleton()
        {
            if (Instance != null)
                throw new Exception("You cannot create more than one instance of MvxSingleton");

            Instance = this as T;
        }

        public static T Instance { get; private set; }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Instance = null;
            }

        }
    }
}
