using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices;

namespace Transportation.Demo.Shared.Interfaces
{
    // Interface for creating and managing IOT Hub jobs
    public interface ITransportationJobClient
    {
        Task<string> StartTwinUpdateJobAsync(string query, Twin twin, DateTime startTime);
        Task<JobStatus> GetJobStatusAsync(string JobId);
        Task<JobStatus> MonitorJobStatusAsync(string JobId);
    }
}
