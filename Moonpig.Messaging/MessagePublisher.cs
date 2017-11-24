namespace Moonpig.Messaging
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public interface IMessagePublisher
    {
        Task Publish<T>(T message);
    }

    public class MessagePublisher : IDisposable, IMessagePublisher
    {
        private readonly ITopicClientFactory _topicClientFactory;
        private readonly IServiceBusConfiguration _serviceBusConfiguration;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private ITopicClient _topicClient;

        public MessagePublisher(ITopicClientFactory topicClientFactory, IServiceBusConfiguration serviceBusConfiguration)
        {
            _topicClientFactory = topicClientFactory;
            _serviceBusConfiguration = serviceBusConfiguration;
            _jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }

        public async Task Publish<T>(T message)
        {
            var brokeredMessage = BuildBrokeredMessage(message);
            await TopicClient.SendAsync(brokeredMessage);
        }

        private Message BuildBrokeredMessage<T>(T message)
        {
            string str = JsonConvert.SerializeObject(message, _jsonSerializerSettings);

            byte[] mBytes = Encoding.UTF8.GetBytes(str);
            var brokeredMessage = new Message
            {
                ContentType = "application/vnd.masstransit+json",
                MessageId = Guid.NewGuid().ToString(),
                TimeToLive = _serviceBusConfiguration.MessageTimeToLive,
                Body = mBytes,
            };

            return brokeredMessage;
        }

        public void Dispose()
        {
            TopicClient.CloseAsync();
        }

        private ITopicClient TopicClient
        {
            get
            {
                if (_topicClient == null)
                {
                    _topicClient = _topicClientFactory.Create();
                }
                return _topicClient;
            }
        }
    }
}
