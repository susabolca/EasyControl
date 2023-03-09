using SCSSdkClient.Object;
using System;

//TODO: possible idea: check if ets is running and if not change update rate to infinity (why most of the user may not quit the application while ets is running)
namespace SCSSdkClient
{
    public delegate void TelemetryData(SCSTelemetry data, bool newTimestamp);

    /// <summary>
    ///     Handle the SCSSdkTelemetry.
    ///     Currently IDisposable. Was implemented because of an error
    /// </summary>
    public class SCSSdkTelemetry
    {
        private const string DefaultSharedMemoryMap = "Local\\SCSTelemetry";

        public bool LinkReady { get; private set; } = false;

        private SharedMemory SharedMemory;

        public SCSSdkTelemetry() => Setup(DefaultSharedMemoryMap);

        public SCSSdkTelemetry(string map) => Setup(map);

        public string Map { get; private set; }

        public Exception Error { get; private set; }

        /// <summary>
        ///     Set up SCS telemetry provider.
        ///     Connects to shared memory map, sets up timebase.
        /// </summary>
        /// <param name="map">Memory Map location</param>
        /// <param name="interval">Timebase interval</param>
        private void Setup(string map)
        {
            LinkReady = false;
            Map = map;

            SharedMemory = new SharedMemory();
            SharedMemory.Connect(map);

            if (!SharedMemory.Hooked)
            {
                Error = SharedMemory.HookException;
                return;
            }

            LinkReady = true;
        }

        public SCSTelemetry Update()
        {
            return SharedMemory.Update<SCSTelemetry>();
        }
    }
}
