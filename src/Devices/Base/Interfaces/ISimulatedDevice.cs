using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transportation.Demo.Devices.Base.Interfaces
{
    public interface ISimulatedDevice
    {
        Task InitializeAsync();
        void StartAllEvents();
        void StopAllEvents();

        void SendMessageToCloud(string messageString);

    }
}
