using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Azimecha.Stupidchat.Core;

namespace Azimecha.Stupidchat.Server {
    public class ThreadExitCallback : IDisposable {
        private Action<Thread> _procOnExit;
        private ExitWaiter _waiter;

        public ThreadExitCallback(Thread thd, Action<Thread> procOnExit) {
            _procOnExit = procOnExit;
            _waiter = GetWaiter(thd);
            _waiter.AddCallback(this);
        }

        public void Dispose() {
            _waiter.RemoveCallback(this);
        }

        ~ThreadExitCallback() {
            _waiter.RemoveCallback(this);
        }

        private static Dictionary<Thread, ExitWaiter> _dicWaiters = new Dictionary<Thread, ExitWaiter>();

        private static ExitWaiter GetWaiter(Thread thd) {
            lock (_dicWaiters) {
                ExitWaiter waiter;
                if (_dicWaiters.TryGetValue(thd, out waiter))
                    return waiter;

                waiter = new ExitWaiter(thd);
                _dicWaiters.Add(thd, waiter);
                return waiter;
            }
        }

        private class ExitWaiter {
            private HashSet<WeakReference<ThreadExitCallback>> _setCallbacks = new HashSet<WeakReference<ThreadExitCallback>>(new CallbackRefComparer());
            private Thread _thdWaitFor;

            public ExitWaiter(Thread thdWaitFor) {
                _thdWaitFor = thdWaitFor;
                Task.Run(WaitTaskProc);
            }

            private void WaitTaskProc() {
                _thdWaitFor.Join();

                lock (_setCallbacks) {
                    foreach (WeakReference<ThreadExitCallback> weakCallback in _setCallbacks)
                        weakCallback.GetValue()?._procOnExit(_thdWaitFor);
                }
            }

            public void AddCallback(ThreadExitCallback callback) {
                lock (_setCallbacks)
                    _setCallbacks.Add(new WeakReference<ThreadExitCallback>(callback));
            }

            public void RemoveCallback(ThreadExitCallback callback) {
                lock (_setCallbacks)
                    _setCallbacks.Remove(new WeakReference<ThreadExitCallback>(callback));
            }
        }

        private class CallbackRefComparer : IEqualityComparer<WeakReference<ThreadExitCallback>> {
            public bool Equals(WeakReference<ThreadExitCallback> x, WeakReference<ThreadExitCallback> y)
                => ReferenceEquals(x.GetValue(), y.GetValue());

            public int GetHashCode(WeakReference<ThreadExitCallback> obj)
                => obj.GetValue()?.GetHashCode() ?? 0;
        }
    }
}
