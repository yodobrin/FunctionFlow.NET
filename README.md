# FunctionFlow.NET

## High Level Architecture

![art](https://user-images.githubusercontent.com/37622785/78224953-10153180-74d2-11ea-9271-d63038b8ccbc.png)

## Narrative Description
Consider a flow of processingm requiring multi steps, each step may have diffrent set of compute requirments. You want to leverage the serverless compute, and to ensure messages are handled without specific order, but with promise to complete.
The process start by sending to the ``` StartFunction ``` an http request. it expect a post message with JSON body. it will then create and ``` EventGridEvent ``` with the content and specific message meta-data and publish it to a custom topic.
```F1``` is triggered by messages sent to that topic, this function is doing its compute and upon completion creates another ``` EventGridEvent ``` with altered message and diffrent meta-data to a second custom topic. ```F2``` is performing a simple upload of the message to a blob, making a footprint.

## Required Resources
The solution use:
- Storage Account - used by functions & the sink container (where all blobs are saved)
- 2 custome Event Grid topics
- Function App (created from vs code)
- Consumption plan (created as part of the function app)
- Application Insights

## Deployment 

### Azure
- Create a resource group for you assests
- Create a storage account (keep track of the connection string) and a container for your sink
- Create two custom event grid topics, note the end point and key per topic
- Edit your local setting file with the above keys, connection string and end-points (provided a demo local setting file)

### Local machine
- clone the repo
- change directory to the newly created folder
- ```code .```
- edit your local settings file with the values from previous step
- using the azure extention, deploy your function to azure, use the ```Advanced``` option when creating it, leverage the priously created resources (storage, application insights, resource group). use a linux consumption plan.
- in case missing libararies use ```dotnet add package <missing package>``` from your vs code terminal
- use ```func start --build``` to run it locally, note you will be able to test your http trigger function 