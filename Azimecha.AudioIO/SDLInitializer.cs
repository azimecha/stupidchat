using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    internal static class SDLInitializer {
        private static readonly object _objInitMutex = new object();
        private static bool _bInited = false;

        public static void EnsureInitialized() {
            if (!_bInited) {
                lock (_objInitMutex) {
                    if (!_bInited) {
                        if (SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_AUDIO) != 0)
                            throw new AudioIOException(SDL2.SDL.SDL_GetError());
                        _bInited = true;
                    }
                }
            }
        }
    }
}
