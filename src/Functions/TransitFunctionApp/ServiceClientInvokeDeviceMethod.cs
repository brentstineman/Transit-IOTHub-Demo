using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TransitFunctionApp
{
    // an implementation of IInvokeDeviceMethod using Service Client
    class ServiceClientInvokeDeviceMethod : IInvokeDeviceMethod
    {
        ServiceClient serviceClient;
        public ServiceClientInvokeDeviceMethod(ServiceClient client)
        {
            serviceClient = client;
        }

        public Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(string deviceId, CloudToDeviceMethod cloudToDeviceMethod)
        {
            return serviceClient.InvokeDeviceMethodAsync(deviceId, cloudToDeviceMethod);
        }

        public Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(string deviceId, CloudToDeviceMethod cloudToDeviceMethod, CancellationToken cancellationToken)
        {
            return serviceClient.InvokeDeviceMethodAsync(deviceId, cloudToDeviceMethod, cancellationToken);
        }

        public Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(string deviceId, string moduleId, CloudToDeviceMethod cloudToDeviceMethod)
        {
            return serviceClient.InvokeDeviceMethodAsync(deviceId, moduleId, cloudToDeviceMethod);
        }

        public Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(string deviceId, string moduleId, CloudToDeviceMethod cloudToDeviceMethod, CancellationToken cancellationToken)
        {
            return serviceClient.InvokeDeviceMethodAsync(deviceId, moduleId, cloudToDeviceMethod, cancellationToken);
        }
    }
}
