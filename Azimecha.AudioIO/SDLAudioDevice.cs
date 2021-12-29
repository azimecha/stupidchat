using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Azimecha.AudioIO {
    internal abstract class SDLAudioDevice : IAudioDevice {
        private uint _nDeviceID;
        private string _strDevName;
        private SDL.SDL_AudioSpec _spec;
        private bool _bRunning, _bPaused;
        private object _objStartStopMutex;
        private SDL.SDL_AudioCallback _callback;

        protected SDLAudioDevice(string strName) {
            _strDevName = strName;
            _bPaused = true;
            _callback = new SDL.SDL_AudioCallback(CallbackWrapper);
            _objStartStopMutex = new object();
            SDLInitializer.EnsureInitialized();
        }

        protected abstract bool IsCaptureDevice { get; }
        protected abstract void AudioCallback(Span<byte> spanBuffer);

        public void Start(SampleFormat format, int nSampleRate, int nChannels, int nSamplesPerChunk, AllowedChanges allowed = AllowedChanges.None) {
            SDL.SDL_AudioSpec specDesired = new SDL.SDL_AudioSpec() {
                freq = nSampleRate,
                channels = (byte)nChannels,
                samples = (ushort)nSamplesPerChunk,
                callback = _callback
            };

            switch (format) {
                case SampleFormat.Signed8Bit:
                    specDesired.format = SDL.AUDIO_S8;
                    break;

                case SampleFormat.Unsigned8Bit:
                    specDesired.format = SDL.AUDIO_U8;
                    break;

                case SampleFormat.Signed16BitLittleEndian:
                    specDesired.format = SDL.AUDIO_S16LSB;
                    break;

                case SampleFormat.Signed16BitBigEndian:
                    specDesired.format = SDL.AUDIO_S16MSB;
                    break;

                case SampleFormat.Signed16BitNative:
                    specDesired.format = SDL.AUDIO_S16SYS;
                    break;

                case SampleFormat.Signed32BitLittleEndian:
                    specDesired.format = SDL.AUDIO_S32LSB;
                    break;

                case SampleFormat.Signed32BitBigEndian:
                    specDesired.format = SDL.AUDIO_S32MSB;
                    break;

                case SampleFormat.Signed32BitNative:
                    specDesired.format = SDL.AUDIO_S32SYS;
                    break;

                case SampleFormat.Floating32BitLittleEndian:
                    specDesired.format = SDL.AUDIO_F32LSB;
                    break;

                case SampleFormat.Floating32BitBigEndian:
                    specDesired.format = SDL.AUDIO_F32MSB;
                    break;

                case SampleFormat.Floating32BitNative:
                    specDesired.format = SDL.AUDIO_F32SYS;
                    break;

                default:
                    throw new NotSupportedException($"{nameof(SDLAudioDevice)} does not support {format}");
            }

            lock (_objStartStopMutex) {
                if (_bRunning)
                    throw new InvalidOperationException("Audio device has already been started");

                _nDeviceID = (_strDevName is null) ? SDL_OpenAudioDevice(IntPtr.Zero, IsCaptureDevice ? 1 : 0, ref specDesired, out _spec, (int)allowed) 
                    : SDL.SDL_OpenAudioDevice(_strDevName, IsCaptureDevice ? 1 : 0, ref specDesired, out _spec, (int)allowed);

                if (_nDeviceID == 0) throw new AudioIOException(SDL.SDL_GetError());

                _bRunning = true;
            }

            Paused = false;
        }

        // the osu nuget package, which we have to use to get the native shared libraries, seems to be missing a way to open the null-named device
        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe uint SDL_OpenAudioDevice(
            IntPtr device,
            int iscapture,
            ref SDL.SDL_AudioSpec desired,
            out SDL.SDL_AudioSpec obtained,
            int allowed_changes
        );

        private unsafe void CallbackWrapper(IntPtr pUserData, IntPtr pBuffer, int nBufLen) {
            try {
                Span<byte> spanBuffer = new Span<byte>((byte*)pBuffer, nBufLen);
                AudioCallback(spanBuffer);
            } catch (Exception ex) {
                Debug.WriteLine($"[{typeof(SDLAudioDevice).FullName}/{nameof(CallbackWrapper)}] Exception in audio callback! {ex}");
            }
        }

        public bool Paused {
            get => !_bRunning || _bPaused;
            set {
                lock (_objStartStopMutex) {
                    if (!_bRunning)
                        throw new InvalidOperationException("Cannot pause an audio device that isn't running");

                    SDL.SDL_PauseAudioDevice(_nDeviceID, value ? 1 : 0);
                    _bPaused = value;
                }
            }
        }

        public SampleFormat Format {
            get {
                switch (_spec.format) {
                    case SDL.AUDIO_S8:
                        return SampleFormat.Signed8Bit;

                    case SDL.AUDIO_U8:
                        return SampleFormat.Unsigned8Bit;

                    case SDL.AUDIO_S16LSB:
                        return SampleFormat.Signed16BitLittleEndian;

                    case SDL.AUDIO_S16MSB:
                        return SampleFormat.Signed16BitBigEndian;

                    case SDL.AUDIO_U16LSB:
                        return SampleFormat.Unsigned16BitLittleEndian;

                    case SDL.AUDIO_U16MSB:
                        return SampleFormat.Unsigned16BitBigEndian;

                    case SDL.AUDIO_S32LSB:
                        return SampleFormat.Signed32BitLittleEndian;

                    case SDL.AUDIO_S32MSB:
                        return SampleFormat.Signed32BitBigEndian;

                    case SDL.AUDIO_F32LSB:
                        return SampleFormat.Floating32BitLittleEndian;

                    case SDL.AUDIO_F32MSB:
                        return SampleFormat.Floating32BitBigEndian;

                    default:
                        return SampleFormat.Unknown;
                }
            }
        }

        public int SampleRate => _spec.freq;
        public int Channels => _spec.channels;
        public int SamplesPerChunk => _spec.samples;

        protected virtual void Dispose(bool bDisposing) {
            if (_bRunning) {
                lock (_objStartStopMutex) {
                    if (_bRunning) {
                        SDL.SDL_CloseAudioDevice(_nDeviceID);
                        _bRunning = false;
                        _bPaused = true;
                    }
                }
            }
        }

        ~SDLAudioDevice() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(bDisposing: false);
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(bDisposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
