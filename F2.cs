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
using Azure.Storage.Blobs;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace FunctionFlow.NET
{
    public static class F2
    {
        [FunctionName("F2")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("SINK_CONTAINER");
            string content = eventGridEvent.Data.ToString();
            string filename = $"mess{Guid.NewGuid().ToString()}";

            // create some footprint 
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);     
            BlobClient blobClient = containerClient.GetBlobClient(filename);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(content));

            try{
                await blobClient.UploadAsync(stream);        
            }catch(Exception ex)
            {
                log.LogInformation($"Exception found {ex.Message}");
            }
                

            log.LogInformation($"F2 was called and saved a blob {filename}");
        }
    }
}
