# FunctionFlow.NET

## High Level Architecture

![art](https://user-images.githubusercontent.com/37622785/78224953-10153180-74d2-11ea-9271-d63038b8ccbc.png)

## Narrative Description
Consider a flow of processingm requiring multi steps, each step may have diffrent set of compute requirments. You want to leverage the serverless compute, and to ensure messages are handled without specific order, but with promise to complete.
The process start by sending to the ``` StartFunction ``` an http request. it expect a post message with JSON body. it will then create and ``` EventGridEvent ``` with the content and specific message meta-data and publish it to a custom topic.
```F1``` is triggered by messages sent to that topic, this function is doing its compute and upon completion creates another ``` EventGridEvent ``` with altered message and diffrent meta-data to a second custom topic. ```F2``` is performing a simple upload of the message to a blob, making a footprint.