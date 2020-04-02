/*
Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.
THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code, provided that. 
You agree: 
	(i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded;
    (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and
	(iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code
**/

// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.EventGrid;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FunctionFlow.NET
{
    public static class F1
    {
        [FunctionName("F1")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            string message = $"F1 got message {eventGridEvent.Data.ToString()}";
            log.LogInformation(message);

            string topicEndpoint  = Environment.GetEnvironmentVariable("EG_SECOND_EP");
            string topicKey  = Environment.GetEnvironmentVariable("EG_SECOND_KEY");

            // do some procssing 

            // pass notification to the next handler
            EventGridEvent mess = GetEvent(message);

            // get a connection
            string topicHostname = new Uri(topicEndpoint).Host;
            TopicCredentials topicCredentials = new TopicCredentials(topicKey);
            EventGridClient client = new EventGridClient(topicCredentials);

            List<EventGridEvent> eventsList = new List<EventGridEvent>();
            eventsList.Add(mess);
            try{
                await client.PublishEventsAsync(topicHostname,eventsList);
            }catch(Exception ex)
            {
               log.LogInformation($"Exception found {ex.Message}"); 
            }
            
        }

        private static EventGridEvent GetEvent(dynamic data)
        {
            EventGridEvent temp = new EventGridEvent();
            temp.Id = Guid.NewGuid().ToString();
            temp.Data = data;        
            temp.Subject = "F1";
            temp.EventTime = DateTime.Now;
            temp.EventType = "Function.Flow.2F2";
            temp.DataVersion = "2.0";            
            return temp;
        }
    }
}
