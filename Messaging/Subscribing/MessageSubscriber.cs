namespace Messaging.Subscribing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using Microsoft.Azure.ServiceBus;
	using Newtonsoft.Json;

	public interface IMessageSubscriber: IDisposable
    {
        void Subscribe<T>(Action<T> action);
    }

    public class MessageSubscriber : IMessageSubscriber
    {
        private readonly IServiceBusConfiguration _serviceBusConfiguration;
        readonly Dictionary<string, SubscriptionClient> _subscriptionClients = new Dictionary<string, SubscriptionClient>();

        public MessageSubscriber(IServiceBusConfiguration serviceBusConfiguration)
        {
            _serviceBusConfiguration = serviceBusConfiguration;
        }

        public void Subscribe<T>(Action<T> action)
        {
            var topicName = typeof(T).FullName;
            var subscriptionName = Assembly.GetEntryAssembly().FullName.Split(',').First();
            var key = $"{topicName}:{subscriptionName}";
            if (_subscriptionClients.ContainsKey(key))
            {
                return;
            }

            var subscriptionClient = new SubscriptionClient(_serviceBusConfiguration.ConnectionString, topicName, subscriptionName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process messages
            subscriptionClient.RegisterMessageHandler((_, __) => ProcessMessagesAsync(_, __, action), messageHandlerOptions);

            _subscriptionClients.Add(key, subscriptionClient);
        }

        public async Task ProcessMessagesAsync<T>(Message message, CancellationToken token, Action<T> action)
        {

            var topicName = typeof(T).FullName;
            var subscriptionName = Assembly.GetEntryAssembly().FullName.Split(',').First();
            var key = $"{topicName}:{subscriptionName}";
            if (!_subscriptionClients.ContainsKey(key))
            {
                return;
            }

            T obj = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));
            action(obj);
            //Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is default).
            await _subscriptionClients[key].CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the subscriptionClient has already been closed.
            // If subscriptionClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.
        }

        // Use this Handler to look at the exceptions received on the MessagePump
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            foreach (var subscriptionClient in _subscriptionClients)
            {
                subscriptionClient.Value.CloseAsync().Wait();
            }
        }
    }
}
