using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Transportation.Demo.Shared;
using Transportation.Demo.Shared.Interfaces;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System.Threading;

namespace Transportation.Demo.Shared
{
    /// <summary>
    /// This class helps you interact with the IOT Hub jobs
    /// </summary>
    public class TransportationJobClient : ITransportationJobClient
    {
        private static JobClient jobClient;

        public TransportationJobClient(string connectionString)
        {
            jobClient = JobClient.CreateFromConnectionString(connectionString);
        }

        public Task<JobStatus> GetJobStatusAsync(string JobId)
        {
            var jobStatus = jobClient.GetJobAsync(JobId).Result;

            return Task.Run(() => jobStatus.Status);
        }

        public Task<JobStatus> MonitorJobStatusAsync(string JobId)
        {
            JobStatus jobStatus = JobStatus.Unknown;

            // monitor the job until it completes
            do
            {
                if (jobStatus != JobStatus.Unknown) // if this isn't our first run
                {
                    Thread.Sleep(2000); // wait 2 seconds before trying again
                }

                // get current job status
                jobStatus = this.GetJobStatusAsync(JobId).Result;
            } while ((jobStatus != JobStatus.Completed) &&  (jobStatus != JobStatus.Failed));

            return Task.Run(() => jobStatus);
        }

        public Task<string> StartTwinUpdateJobAsync(string query, Twin twin, DateTime startTime)
        {
            string JobId = Guid.NewGuid().ToString();

            JobResponse createJobResponse = jobClient.ScheduleTwinUpdateAsync(
              JobId, query, twin, DateTime.UtcNow,
              (long)TimeSpan.FromMinutes(2).TotalSeconds).Result;

            return Task.Run(() => JobId);
        }
    }
}