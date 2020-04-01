// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;

namespace FunctionFlow.NET
{
    public static class F1
    {
        [FunctionName("F1")]
        public static async void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            string message = $"F1 got message {eventGridEvent.Data.ToString()}";
            log.LogInformation(message);

            string topicEndpoint  = Environment.GetEnvironmentVariable("EG_SECOND_EP");
            string topicKey  = Environment.GetEnvironmentVariable("EG_SECOND_KEY");

            EventGridEvent mess = GetEvent(message);

            // get a connection
            string topicHostname = new Uri(topicEndpoint).Host;
            TopicCredentials topicCredentials = new TopicCredentials(topicKey);
            EventGridClient client = new EventGridClient(topicCredentials);

            List<EventGridEvent> eventsList = new List<EventGridEvent>();
            eventsList.Add(mess);

            await client.PublishEventsAsync(topicHostname,eventsList);

        }

        private static EventGridEvent GetEvent(dynamic data)
        {
            EventGridEvent temp = new EventGridEvent();
            temp.Id = Guid.NewGuid().ToString();
            temp.Data = data;
            return temp;
        }
    }
}
