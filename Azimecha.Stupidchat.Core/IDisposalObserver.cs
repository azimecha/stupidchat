using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public interface IDisposalObserver<T> {
        void OnObjectDisposed(T obj);
    }
}
