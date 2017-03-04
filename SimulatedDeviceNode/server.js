var azure = require('azure');

var serviceBusService = azure.createServiceBusService('Endpoint=sb://servicebus-ipowersaver-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rI3tGgKoxeizB6P4BZ9skQZvHcMOCX4N7tHcHmyre8A=')

serviceBusService.createTopicIfNotExists('datapoint', function (error) {
    if (!error) {
        // Topic was created or exists
        console.log('topic created or exists.');
    }
});

var topicOptions = {
    MaxSizeInMegabytes: '5120',
    DefaultMessageTimeToLive: 'PT1M'
};

serviceBusService.createSubscription('datapoint', 'AllMessages', function (error) {
    if (!error) {
        console.log('success');
    } else {
        console.log(error);
    }
});


setInterval(function () {
    serviceBusService.receiveSubscriptionMessage('datapoint', 'AllMessages', function (error, receivedMessage) {
        if (!error) {
            // Message received and deleted
            console.log(receivedMessage);
        } else {
            console.log(error);
        }
    })
}, 1000);

