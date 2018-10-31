using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TransitFunctionApp
{
    // interface to describe the dependency that PurchaseTicketAction needs from ServiceClient
    public interface IInvokeDeviceMethod
    {
        Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(
            string deviceId,
            CloudToDeviceMethod cloudToDeviceMethod);

        Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(
            string deviceId,
            CloudToDeviceMethod cloudToDeviceMethod,
            CancellationToken cancellationToken);

        Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(
            string deviceId,
            string moduleId,
            CloudToDeviceMethod cloudToDeviceMethod);

        Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(
            string deviceId,
            string moduleId,
            CloudToDeviceMethod cloudToDeviceMethod,
            CancellationToken cancellationToken);
    }
}
