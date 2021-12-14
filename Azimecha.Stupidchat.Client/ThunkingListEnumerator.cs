using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Client {
    public abstract class ThunkingListEnumerator<T> : IEnumerator<T> {
        private IList<T> _lstToEnumerate;
        private int _nCurIndex;

        public ThunkingListEnumerator(IList<T> lstToEnumerate) {
            _lstToEnumerate = lstToEnumerate;
            _nCurIndex = -1;
        }

        public T Current => _lstToEnumerate[_nCurIndex];
        object IEnumerator.Current => _lstToEnumerate[_nCurIndex];

        public void Dispose() {
            _lstToEnumerate = null;
            _nCurIndex = 0;
        }

        public bool MoveNext() {
            _nCurIndex++;
            
            if (_nCurIndex > _lstToEnumerate.Count)
                Thunk();

            if (_nCurIndex > _lstToEnumerate.Count)
                return false;

            return true;
        }

        public void Reset() {
            _nCurIndex = -1;
        }

        protected abstract void Thunk();
    }


    public class ThunkingListEnumeratorFactory<TVal, TEnu> : IEnumerable<TVal> where TEnu : ThunkingListEnumerator<TVal> {
        private IList<TVal> _lstToEnumerate;

        public ThunkingListEnumeratorFactory(IList<TVal> lstToEnumerate) {
            _lstToEnumerate = lstToEnumerate;
        }

        public IEnumerator<TVal> GetEnumerator() => (TEnu)Activator.CreateInstance(typeof(TEnu), _lstToEnumerate);
        IEnumerator IEnumerable.GetEnumerator() => (TEnu)Activator.CreateInstance(typeof(TEnu), _lstToEnumerate);
    }
}
