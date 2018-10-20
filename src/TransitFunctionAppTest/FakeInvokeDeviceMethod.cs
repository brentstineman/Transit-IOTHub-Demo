using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransitFunctionApp;

namespace TransitFunctionAppTest
{
    // Fake implementation for IInvokeDeviceMethod that logs incoming invocations
    public class FakeInvokeDeviceMethod : IInvokeDeviceMethod
    {
        public struct InvocationInfo
        {
            public string device;
            public string module;
            public CloudToDeviceMethod method;
        }
        public List<InvocationInfo> invocations;
        public FakeInvokeDeviceMethod()
        {
            invocations = new List<InvocationInfo>();
        }
        public Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(string deviceId, CloudToDeviceMethod cloudToDeviceMethod)
        {
            invocations.Add(new InvocationInfo() { device = deviceId, module = "", method = cloudToDeviceMethod });
            // TODO return a result...
            return Task<CloudToDeviceMethodResult>.Factory.StartNew(() => null);
        }

        public Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(string deviceId, CloudToDeviceMethod cloudToDeviceMethod, CancellationToken cancellationToken)
        {
            invocations.Add(new InvocationInfo() { device = deviceId, module = "", method = cloudToDeviceMethod });
            // TODO return a result...
            return Task<CloudToDeviceMethodResult>.Factory.StartNew(() => null);
        }

        public Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(string deviceId, string moduleId, CloudToDeviceMethod cloudToDeviceMethod)
        {
            invocations.Add(new InvocationInfo() { device = deviceId, module = moduleId, method = cloudToDeviceMethod });
            // TODO return a result...
            return Task<CloudToDeviceMethodResult>.Factory.StartNew(() => null);
        }

        public Task<CloudToDeviceMethodResult> InvokeDeviceMethodAsync(string deviceId, string moduleId, CloudToDeviceMethod cloudToDeviceMethod, CancellationToken cancellationToken)
        {
            invocations.Add(new InvocationInfo() { device = deviceId, module = moduleId, method = cloudToDeviceMethod });
            // TODO return a result...
            return Task<CloudToDeviceMethodResult>.Factory.StartNew(() => null);
        }
    }
}
