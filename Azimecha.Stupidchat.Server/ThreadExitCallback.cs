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
        private static Dictionary<Thread, ExitWaiter> _dicWaiters = new Dictionary<Thread, ExitWaiter>();
        private static long _nNextUniqueID = 0;

        private WeakReference<ExitWaiter> _waiter;
        private Action<Thread> _procOnExit;
        private long _nUniqueID;

        public ThreadExitCallback(Thread thd, Action<Thread> procOnExit) {
            _nUniqueID = Interlocked.Increment(ref _nNextUniqueID);
        }

        public void Dispose() {
            _waiter.GetValue()?.RemoveCallback(_nUniqueID);
        }

        private class ExitWaiter {
            private Task _taskWaitForExit;
            private ConcurrentDictionary<long, WeakReference<ThreadExitCallback>> _dicCallbacks;
            private Thread _thdToWatch;

            public ExitWaiter(Thread thd) {
                _thdToWatch = thd;
                _dicCallbacks = new ConcurrentDictionary<long, WeakReference<ThreadExitCallback>>();
                _taskWaitForExit = Task.Run(WaitTaskProc);
            }

            private void WaitTaskProc() {
                _thdToWatch.Join();

                while (_dicCallbacks.Count > 0) {
                    long[] arrKeys = _dicCallbacks.Keys.ToArray();
                    foreach (long nKey in arrKeys) {
                        WeakReference<ThreadExitCallback> weakCallback;
                        if (_dicCallbacks.TryGetValue(nKey, out weakCallback)) {
                            ThreadExitCallback callback;
                            if (weakCallback.TryGetTarget(out callback))
                                callback._procOnExit.Invoke(_thdToWatch);
                        }
                    }
                }

                _taskWaitForExit = null;
            }

            public void AddCallback(ThreadExitCallback cb) {
                _dicCallbacks.TryAdd(cb._nUniqueID, new WeakReference<ThreadExitCallback>(cb));
            }

            public void RemoveCallback(long nUniqueID) {
                WeakReference<ThreadExitCallback> weakDummy;
                _dicCallbacks.TryRemove(nUniqueID, out weakDummy);
            }
        }
    }
}
