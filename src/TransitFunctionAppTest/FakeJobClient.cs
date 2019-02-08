using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Transportation.Demo.Shared.Interfaces;

namespace TransportationDemoTests
{
    /// <summary>
    /// This class is used to help test code that is dependent on the TransportationJobClient
    /// </summary>
    public class FakeJobClient : ITransportationJobClient
    {
        public List<string> sendMessageLog = new List<string>();
        private Twin fakeTwin;

        public FakeJobClient(Twin deviceTwin = null)
        {
            this.fakeTwin = deviceTwin ?? new Microsoft.Azure.Devices.Shared.Twin("fakeDevice");
        }

        public Task<JobStatus> GetJobStatusAsync(string JobId)
        {
            return Task.Run(() => JobStatus.Completed);
        }

        public Task<JobStatus> MonitorJobStatusAsync(string JobId)
        {
            return Task.Run(() => JobStatus.Completed);
        }

        public Task<string> StartTwinUpdateJobAsync(string query, Twin twin, DateTime startTime)
        {
            string JobId = new Guid().ToString();
            fakeTwin = twin;

            return Task.Run(() => JobId);
        }
    }
}
