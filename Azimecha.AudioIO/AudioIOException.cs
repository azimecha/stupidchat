using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    public class AudioIOException : Exception {
        public AudioIOException(string strMessage) : base(strMessage) { }
    }
}
