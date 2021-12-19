using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace Azimecha.Stupidchat.Server {
    public class ThreadLocalLazy<T> : IDisposable {
        private Dictionary<Thread, Instance> _dicInstances;
        private Func<T> _procFactory;
        private Action<T> _procDisposer;

        public ThreadLocalLazy() : this(DefaultFactory, DefaultDisposer) {}
        public ThreadLocalLazy(Func<T> procFactory) : this(procFactory, DefaultDisposer) {}
        public ThreadLocalLazy(Action<T> procDisposer) : this(DefaultFactory, procDisposer) { }

        public ThreadLocalLazy(Func<T> procFactory, Action<T> procDisposer) {
            _dicInstances = new Dictionary<Thread, Instance>();
            _procFactory = procFactory;
            _procDisposer = procDisposer;
        }

        private static T DefaultFactory() => Activator.CreateInstance<T>();
        private static void DefaultDisposer(T obj) => (obj as IDisposable)?.Dispose();

        public void Dispose() {
            lock (_dicInstances) {
                foreach (KeyValuePair<Thread, Instance> kvpInst in _dicInstances)
                    kvpInst.Value.Dispose();
                _dicInstances.Clear();
            }
        }

        public T Value {
            get {
                Instance inst;
                Thread thdCur = Thread.CurrentThread;

                lock (_dicInstances) {
                    if (_dicInstances.TryGetValue(thdCur, out inst))
                        return inst.Object;
                }

                // no race condition: no other thread is the same as this Thread.CurrentThread
                inst = new Instance(this);
                
                lock (_dicInstances)
                    _dicInstances.Add(thdCur, inst);

                return inst.Object;
            }
        }

        public void DiscardMine() => DiscardSpecific(Thread.CurrentThread);

        private void DiscardSpecific(Thread thd) {
            lock (_dicInstances) {
                Instance inst;
                if (_dicInstances.TryGetValue(thd, out inst))
                    inst.Dispose();
            }
        }

        private class Instance : IDisposable {
            private Action<T> _procDisposer;
            private T _object;

            public Instance(ThreadLocalLazy<T> lazy) {
                _object = lazy._procFactory();
                _procDisposer = lazy._procDisposer;
            }

            public T Object => _object;

            public void Dispose() {
                _procDisposer(_object);
            }
        }
    }
}
