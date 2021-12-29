using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    public static class Devices {
        public static IEnumerable<IDeviceFactory<IPlaybackDevice>> PlaybackDevices => new SDLDevicesEnumerable<IPlaybackDevice, SDLPlaybackDevice>(false);
        public static IEnumerable<IDeviceFactory<IRecordingDevice>> RecordingDevices => new SDLDevicesEnumerable<IRecordingDevice, SDLRecordingDevice>(true);
        public static IDeviceFactory<IPlaybackDevice> DefaultPlaybackDevice => new SDLDeviceFactory<IPlaybackDevice, SDLPlaybackDevice>(null);
        public static IDeviceFactory<IRecordingDevice> DefaultRecordingDevice => new SDLDeviceFactory<IRecordingDevice, SDLRecordingDevice>(null);
    }

    internal class SDLDeviceFactory<T, D> : IDeviceFactory<T> where T : IAudioDevice where D : SDLAudioDevice {
        public SDLDeviceFactory(string strDeviceName) { 
            _strName = strDeviceName; 
        }

        private string _strName;
        public string Name => _strName ?? "Default Device";

        public T CreateDevice() => (T)Activator.CreateInstance(type: typeof(D), args: _strName);
    }

    internal class SDLDevicesEnumerable<T, D> : IEnumerable<IDeviceFactory<T>> where T : IAudioDevice where D : SDLAudioDevice {
        public SDLDevicesEnumerable(bool bEnumRecordingDevices) {
            EnumRecordingDevices = bEnumRecordingDevices;
        }

        public bool EnumRecordingDevices { get; private set; }

        public IEnumerator<IDeviceFactory<T>> GetEnumerator() => new SDLDevicesEnumerator<T, D>(EnumRecordingDevices);
        IEnumerator IEnumerable.GetEnumerator() => new SDLDevicesEnumerator<T, D>(EnumRecordingDevices);
    }

    internal class SDLDevicesEnumerator<T, D> : IEnumerator<IDeviceFactory<T>> where T : IAudioDevice where D : SDLAudioDevice {
        private int _nCurrentIndex;
        private int _nCount;

        public SDLDevicesEnumerator(bool bEnumRecordingDevices) {
            EnumRecordingDevices = bEnumRecordingDevices;
            SDLInitializer.EnsureInitialized();
            _nCurrentIndex = -1;
            _nCount = SDL2.SDL.SDL_GetNumAudioDevices(bEnumRecordingDevices ? 1 : 0);
        }

        public bool EnumRecordingDevices { get; private set; }

        public IDeviceFactory<T> Current => ((_nCurrentIndex < 0) || (_nCurrentIndex >= _nCount)) ? null 
            : new SDLDeviceFactory<T, D>(SDL2.SDL.SDL_GetAudioDeviceName(_nCurrentIndex, EnumRecordingDevices ? 1 : 0));

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public bool MoveNext() {
            _nCurrentIndex++;
            return _nCurrentIndex < _nCount;
        }

        public void Reset() {
            _nCount = -1;
        }
    }
}
