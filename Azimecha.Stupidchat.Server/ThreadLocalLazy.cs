using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace Azimecha.Stupidchat.Server {
    public class ThreadLocalLazy<T> : IDisposable {
        private ConcurrentDictionary<Thread, T> _dicInstances;
        private ConcurrentDictionary<Thread, ThreadExitCallback> _dicCallbacks;
        private Func<T> _procFactory;
        private Action<T> _procDisposer;
        private ConcurrentBag<Thread> _bagThreads;

        public ThreadLocalLazy() : this(DefaultFactory, DefaultDisposer) {}
        public ThreadLocalLazy(Func<T> procFactory) : this(procFactory, DefaultDisposer) {}
        public ThreadLocalLazy(Action<T> procDisposer) : this(DefaultFactory, procDisposer) { }

        public ThreadLocalLazy(Func<T> procFactory, Action<T> procDisposer) {
            _dicInstances = new ConcurrentDictionary<Thread, T>();
            _procFactory = procFactory;
            _procDisposer = procDisposer;
        }

        private static T DefaultFactory() => Activator.CreateInstance<T>();
        private static void DefaultDisposer(T obj) => (obj as IDisposable)?.Dispose();

        public void Dispose() {
            Thread thdCur;

            while (_bagThreads.TryTake(out thdCur)) {
                T obj;

                if (_dicInstances.TryRemove(thdCur, out obj))
                    _procDisposer(obj);
            }
        }

        public T Value {
            get {
                Thread thdCur = Thread.CurrentThread;
                T obj;
                ThreadExitCallback callback = null;

                if (_dicInstances.TryGetValue(thdCur, out obj))
                    return obj;

                obj = _procFactory();

                try {
                    callback = new ThreadExitCallback(thdCur, thd => DiscardSpecific(thdCur));

                    if (!_dicInstances.TryAdd(thdCur, obj))
                        throw new InvalidOperationException("Could not get or add value - did another thread use our thead object?");

                    if (!_dicCallbacks.TryAdd(thdCur, callback))
                        throw new InvalidOperationException("Could not get or add value - did another thread use our thead object?");

                } catch (Exception ex) {
                    T objDummy;
                    _dicInstances.TryRemove(thdCur, out objDummy);

                    _procDisposer(obj);
                    callback?.Dispose();
                    throw;
                }


                return obj;
            }
        }

        public void DiscardMine() => DiscardSpecific(Thread.CurrentThread);

        private void DiscardSpecific(Thread thd) {
            T obj;
            if (_dicInstances.TryRemove(thd, out obj))
                _procDisposer(obj);
        }
    }
}
