namespace Messaging.Publishing
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Threading.Tasks;
	using Microsoft.Azure.ServiceBus;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	public interface IMessagePublisher : IDisposable
    {
        Task Publish<T>(T message);
    }

    public class MessagePublisher : IMessagePublisher
    {
        private readonly IServiceBusConfiguration _serviceBusConfiguration;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        readonly Dictionary<string, ITopicClient> _topicClients;

        public MessagePublisher(IServiceBusConfiguration serviceBusConfiguration)
        {
            _serviceBusConfiguration = serviceBusConfiguration;
            _jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            _topicClients = new Dictionary<string, ITopicClient>();
        }

        public async Task Publish<T>(T message)
        {
            var topicName = typeof(T).FullName;
            if (!_topicClients.TryGetValue(topicName, out var topicClient))
            {
                topicClient = Create(topicName);
                _topicClients.Add(topicName, topicClient);
            }

            var brokeredMessage = BuildBrokeredMessage(message);
            await topicClient.SendAsync(brokeredMessage);
        }

        private Message BuildBrokeredMessage<T>(T message)
        {
            string str = JsonConvert.SerializeObject(message, _jsonSerializerSettings);

            byte[] mBytes = Encoding.UTF8.GetBytes(str);
            var brokeredMessage = new Message
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                TimeToLive = _serviceBusConfiguration.MessageTimeToLive,
                Body = mBytes,
            };

            return brokeredMessage;
        }

        public void Dispose()
        {
            foreach (var topicClient in _topicClients)
            {
                topicClient.Value.CloseAsync();
            }
        }

        private ITopicClient Create(string topicName)
        {
            RetryExponential retryPolicy = new RetryExponential(_serviceBusConfiguration.RetryPolicyMinimumBackoff,
                _serviceBusConfiguration.RetryPolicyMaximumBackoff,
                _serviceBusConfiguration.RetryPolicyMaximumRetryCount);

            var topicClient = new TopicClient(_serviceBusConfiguration.ConnectionString, topicName, retryPolicy);

            return topicClient;
        }
    }
}
