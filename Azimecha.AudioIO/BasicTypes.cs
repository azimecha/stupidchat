using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    public enum SampleFormat {
        Unknown,
        Signed8Bit,
        Unsigned8Bit,
        Signed16BitLittleEndian,
        Signed16BitBigEndian,
        Signed16BitNative,
        Unsigned16BitLittleEndian,
        Unsigned16BitBigEndian,
        Unsigned16BitNative,
        Signed32BitLittleEndian,
        Signed32BitBigEndian,
        Signed32BitNative,
        Floating32BitLittleEndian,
        Floating32BitBigEndian,
        Floating32BitNative
    }

    public static class SampleRate {
        public const int CD_QUALITY = 44100;
        public const int STANDARD = 48000;
    }

    public static class ChannelCount {
        public const int MONO = 1;
        public const int STEREO = 2;
        public const int QUAD = 4;
        public const int FIVE_POINT_ONE = 6;
    }

    [Flags]
    public enum AllowedChanges {
        None = 0,
        Frequency = 1 << 0,
        Format = 1 << 1,
        Channels = 1 << 2,
        Any = Frequency | Format | Channels
    }
}
