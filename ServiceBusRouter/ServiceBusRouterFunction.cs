using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;

namespace ServiceBusRouter
{
    public static class ServiceBusRouterFunction
    {
        private static readonly IReadOnlyDictionary<string, string> s_countyToCorrelationId =
            new Dictionary<string, string>
            {
                { "input", "output" },
            };
           
        [FunctionName("ServiceBusRouterFunction")]
        [return: ServiceBus("outtopic", Connection = "MyServiceBusConnection")]
        public static Message Run(
            [ServiceBusTrigger("myqueue", Connection = "MyServiceBusConnection")]
            Message inputMessage, 
            TraceWriter log)
        {
            var inputCorrelationId = inputMessage.CorrelationId;
            log.Info($"Received input CorrelationId: '{inputCorrelationId}'");
            var outputCorrelationId = GetCorrelationId(inputCorrelationId);
            if (outputCorrelationId != null)
            {
                var outputMessage = inputMessage.Clone();
                outputMessage.CorrelationId = outputCorrelationId;
                log.Info($"Forwarding the message with CorrelationId: '{outputCorrelationId}'");
                return outputMessage;
            }
            else
            {
                log.Warning($"Could not retrieve output CorrelationId for: '{inputCorrelationId}'");
                log.Warning($"Dropping message: '{inputMessage.MessageId}'");
                return null;
            }
        }

        private static string GetCorrelationId(string inputCorrelationId)
        {
            if (inputCorrelationId != null &&
                s_countyToCorrelationId.TryGetValue(inputCorrelationId, out var outputCorrelationId))
            {
                return outputCorrelationId;
            }

            return null;
        }
    }
}
