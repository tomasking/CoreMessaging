namespace Moonpig.Messaging
{
    using Microsoft.Azure.ServiceBus;

    public class TopicClientFactory : ITopicClientFactory
    {
        private readonly IServiceBusConfiguration _serviceBusConfiguration;
        private ITopicClient _topicClient;

        public TopicClientFactory(IServiceBusConfiguration serviceBusConfiguration)
        {
            _serviceBusConfiguration = serviceBusConfiguration;
        }

        public ITopicClient Create()
        {
            if (_topicClient == null)
            {
                RetryExponential retryPolicy = new RetryExponential(_serviceBusConfiguration.RetryPolicyMinimumBackoff,
                    _serviceBusConfiguration.RetryPolicyMaximumBackoff,
                    _serviceBusConfiguration.RetryPolicyMaximumRetryCount);

                _topicClient = new TopicClient(_serviceBusConfiguration.ConnectionString, _serviceBusConfiguration.TopicName, retryPolicy);
            }

            return _topicClient;
        }
    }
}