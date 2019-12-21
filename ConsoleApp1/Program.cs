using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    static class Program
    {
        async static Task Main(string[] args)
        {
            var host = args.FirstOrDefault() ?? "localhost";

            using (var serviceHost = new ServiceHost(typeof(Service1)))
            {
                var basicHttpBinding = new BasicHttpBinding();
                var netTcpBinding = new NetTcpBinding();
                var webHttpBinding = new WebHttpBinding();

                var basicHttpEndpoint = serviceHost.AddServiceEndpoint(typeof(IService1), basicHttpBinding, new Uri($"http://{host}/Temporary_Listen_Addresses/service1"));
                var netTcpEndpoint = serviceHost.AddServiceEndpoint(typeof(IService1), netTcpBinding, new Uri("net.tcp://localhost/Temporary_Listen_Addresses/service1"));
                var webHttpEndpoint = serviceHost.AddServiceEndpoint(typeof(IService1), webHttpBinding, new Uri($"http://{host}/Temporary_Listen_Addresses/web/service1"));

                webHttpEndpoint.EndpointBehaviors.Add(new WebHttpBehavior()
                {
                    DefaultOutgoingRequestFormat = WebMessageFormat.Json,
                    DefaultOutgoingResponseFormat = WebMessageFormat.Json,
                });

                serviceHost.Open();

                await CallService(basicHttpBinding, basicHttpEndpoint.Address);
                await CallService(netTcpBinding, netTcpEndpoint.Address);
                await CallService(webHttpBinding, webHttpEndpoint.Address);
            }
        }

        private async static Task CallService(Binding binding, EndpointAddress address)
        {
            using (var channelFactory = new ChannelFactory<IService1Client>(binding, address))
            {
                if (binding is WebHttpBinding)
                {
                    channelFactory.Endpoint.Behaviors.Add(new WebHttpBehavior()
                    {
                        DefaultOutgoingRequestFormat = WebMessageFormat.Json,
                        DefaultOutgoingResponseFormat = WebMessageFormat.Json,
                    });
                }
                using (var channel = channelFactory.CreateChannel())
                {
                    using (var operationContextScope = new OperationContextScope(channel))
                    {
                        EnableAsyncFlow();

                        if (!(binding is WebHttpBinding))
                        {
                            OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(MyHeader.Name, MyHeader.Ns, new MyHeader
                            {
                                SomeData = "Some data"
                            }));
                        }

                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = new HttpRequestMessageProperty()
                        {
                            Headers = { { "x-my-header1", "my-value" } }
                        };

                        var result = await channel.DoWorkAsync();
                        Console.WriteLine(result);
                    }
                }
            }
        }

        private static void EnableAsyncFlow()
        {
            typeof(OperationContext)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .First(mi => mi.Name == nameof(EnableAsyncFlow) && mi.GetParameters().Length == 0)
                .Invoke(null, null);
        }
    }
}
