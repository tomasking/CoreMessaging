namespace Moonpig.Messaging
{
    using System;
    using Publishing;
    using Subscribing;

    public interface IServiceBus : IDisposable
    {
        void Subscribe<T>(Action<T> action);
        void Publish<T>(T testMessage);
    }

    public class ServiceBus : IServiceBus
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageSubscriber _messageSubscriber;

        public ServiceBus(IServiceBusConfiguration serviceBusConfiguration)
        {
            _messagePublisher = new MessagePublisher(serviceBusConfiguration);
            _messageSubscriber = new MessageSubscriber(serviceBusConfiguration);
        }

        public void Subscribe<T>(Action<T> action)
        {
            _messageSubscriber.Subscribe(action);
        }

        public void Publish<T>(T testMessage)
        {
            _messagePublisher.Publish(testMessage);
        }

        public void Dispose()
        {
            _messagePublisher.Dispose();
            _messageSubscriber.Dispose();
        }
    }
}
