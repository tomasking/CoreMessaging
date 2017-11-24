namespace Moonpig.Messaging
{
    using Microsoft.Azure.ServiceBus;

    public interface ITopicClientFactory
    {
        ITopicClient Create();
    }
}