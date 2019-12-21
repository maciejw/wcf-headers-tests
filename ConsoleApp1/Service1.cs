using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Service1 : IService1
    {
        public async Task<string> DoWorkAsync()
        {
            var httpHeaderValue = "";
            var soapHeaderValue = "";

            if (OperationContext.Current.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name, out var value) && value is HttpRequestMessageProperty requestMessageProperty)
            {
                httpHeaderValue = requestMessageProperty.Headers["x-my-header1"];
            }
            var incomingMessageHeaders = OperationContext.Current.IncomingMessageHeaders;

            var headerIndex = incomingMessageHeaders.FindHeader(MyHeader.Name, MyHeader.Ns);
            if (headerIndex >= 0)
            {
                var myHeader = incomingMessageHeaders.GetHeader<MyHeader>(headerIndex);
                soapHeaderValue = myHeader?.SomeData;
            }
            return await Task.FromResult($"{httpHeaderValue}, {soapHeaderValue}");
        }
    }
}
