using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.AudioIO.PlaybackTest {
    public static class PlaybackTest {
        private const int SAMPLE_RATE = 48000;
        private const float TONE_FREQUENCY = 1000.0f;

        public static void Main(string[] arrArgs) {
            foreach (IDeviceFactory<IPlaybackDevice> facDevice in Devices.PlaybackDevices)
                Console.WriteLine($"Playback device: {facDevice.Name}");

            Console.WriteLine($"Default playback device: {Devices.DefaultPlaybackDevice.Name}");
            IPlaybackDevice dev = Devices.DefaultPlaybackDevice.CreateDevice();
            dev.Start(SampleFormat.Signed32BitNative, SAMPLE_RATE, 1, 4096);

            int[] arrData = new int[SAMPLE_RATE];
            AutoResetEvent evtSecondElapsed = new AutoResetEvent(true);

            System.Timers.Timer timer = new System.Timers.Timer(1000.0);
            timer.Elapsed += (_, _) => evtSecondElapsed.Set();
            timer.Start();

            while (true) {
                GenerateSinewave(arrData, TONE_FREQUENCY, SAMPLE_RATE);
                evtSecondElapsed.WaitOne();
                dev.EnqueueData(arrData);
            }
        }

        private static void GenerateSinewave(Span<int> spanOut, float fFrequency, int nSampleRate) {
            for (int nSample = 0; nSample < spanOut.Length; nSample++) {
                float fAngle = ((float)nSample / (float)nSampleRate) * fFrequency * (float)Math.PI * 2;
                spanOut[nSample] = (int)(Math.Sin(fAngle) * int.MaxValue);
            }
        }
    }
}
