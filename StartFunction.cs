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
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;

namespace FunctionFlow.NET
{
    public static class StartFunction
    {
        [FunctionName("StartFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("StartFunction function processed a request.");
            // get connection details from env
            string topicEndpoint  = Environment.GetEnvironmentVariable("EG_START_EP");
            string topicKey  = Environment.GetEnvironmentVariable("EG_START_KEY");

            // get a connection
            string topicHostname = new Uri(topicEndpoint).Host;
            TopicCredentials topicCredentials = new TopicCredentials(topicKey);
            EventGridClient client = new EventGridClient(topicCredentials);

            // perform initial procssing

            // send message to the next handler
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic ddata = JsonConvert.DeserializeObject(requestBody);
            var data = (ddata!=null)?ddata:"i got no message content";
            EventGridEvent mess = GetEvent(data);
            List<EventGridEvent> eventsList = new List<EventGridEvent>();
            eventsList.Add(mess);

            await client.PublishEventsAsync(topicHostname,eventsList);


            string responseMessage = $"sent single message, to this topic: {topicEndpoint}.";

            return new OkObjectResult(responseMessage);
        }

        private static EventGridEvent GetEvent(dynamic data)
        {
            EventGridEvent temp = new EventGridEvent();
            temp.Id = Guid.NewGuid().ToString();
            temp.Data = data;
            temp.Subject = "test1";
            temp.EventTime = DateTime.Now;
            temp.EventType = "Function.Flow.2F1";
            temp.DataVersion = "2.0";
            return temp;
        }

    }
}
