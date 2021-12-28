using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public class TemporaryFile : IDisposable {
        private string _strPath;
        private bool _bDeleted, _bDeleteOnDispose;

        public TemporaryFile() : this(System.IO.Path.GetTempFileName(), true) { }

        public TemporaryFile(string strPath, bool bDeleteOnDispose) {
            _strPath = strPath;
            _bDeleteOnDispose = bDeleteOnDispose;
        }

        public string Path => _strPath;

        public void Detach() {
            _bDeleteOnDispose = false;
        }

        protected virtual void Dispose(bool bDisposing) {
            if (_bDeleteOnDispose && !_bDeleted) {
                try {
                    System.IO.File.Delete(_strPath);
                    _bDeleted = true;
                } catch (System.IO.FileNotFoundException) {
                    // already gone
                    _bDeleted = true;
                } catch (Exception ex) {
                    Debug.WriteLine($"[TemporaryFile] Error deleting {_strPath}: {ex}");
                }
            }
        }

        ~TemporaryFile() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(false);
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(bDisposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
