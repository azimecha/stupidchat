using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public static class Utils {
        public static T GetValue<T>(this WeakReference<T> weak) where T : class {
            T obj;
            return weak.TryGetTarget(out obj) ? obj : null;
        }
    }
}
