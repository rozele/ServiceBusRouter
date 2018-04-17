# Service Bus Router Function

This is a simple sample Azure Function using .NET Standard to connect an Azure Service Bus queue to an Azure Service Bus topic.

The scenario is that some publisher service is decoupled from multiple subscriber services. The publisher service uses an unchanging correlation ID space that is mapped to a dynamic correlation ID space for the downstream subscribers.

The purpose of the Azure Function is to use the correlation ID provided by the publisher to look up a new correlation ID that will route the message to the appropriate subscriber services.

## Getting Started

### Prequisites

* Visual Studio 2017
* Azure Subscription
* Azure Storage Account
* Azure Service Bus Namespace
* Azure Service Bus Queue called `mytopic` in Azure Service Bus Namespace
* Azure Service Bus Topic called `outtopic` in Azure Service Bus Namespace
* Azure Service Bus Subscription in Azure Service Bus Topic

TODO: Add a one-click deploy Azure button to configure the Storage account and the Service Bus.

### Running the Solution

To get started, clone the repository:
```
git clone https://github.com/rozele/ServiceBusRouter
```

Add a new file to the ServiceBusRouter project called `local.settings.json`:
```
cd ServiceBusRouter
cp ./ServiceBusRouter/sample.settings.json ./ServiceBusRouter/local.settings.json
```

In the `local.settings.json` file, add your Azure Storage connection string and your Azure Service Bus connection string:
```
{
    "IsEncrypted": false,
    "Values": {
      "AzureWebJobsStorage": "<storage connection string>",
      "AzureWebJobsDashboard": "<storage connection string>",
      "MyServiceBusConnection": "<service bus connection string>"
    }
  }
```

Open the solution in Visual Studio 2017 and run the solution:
```
open ServiceBusRouter.sln
```

Using your Azure Service Bus publishing tool of choice (we recommend [Service Bus Explorer](https://github.com/paolosalvatori/ServiceBusExplorer)), publish a message to the `myqueue` Azure Service Bus queue you created in the prereqs. Ensure that the message you create uses the `CorrelationId` value `"input"`.

In your running Azure Function app window, you should see the following log messages:
```
Received input CorrelationId: 'input'
Forwarding the message with CorrelationId: 'output'
```

Using your Azure Service Bus subscriber tool of choice (again, consider Service Bus Explorer), view the messages in the topic subscription you created in the prerequisites. You should see the message you originally published with a new `CorrelationId` value `"output"`.
