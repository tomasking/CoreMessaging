using System;

namespace Publisher
{
    using Microsoft.Extensions.Configuration;
    using Moonpig.Messaging;

    class Program
    {
        static void Main(string[] args)
        {
            var serviceBusConfiguration = new ServiceBusConfiguration(new ConfigurationBuilder());
            ITopicClientFactory topicClientFactory = new TopicClientFactory(serviceBusConfiguration);
            IMessagePublisher messagePublisher = new MessagePublisher(topicClientFactory, serviceBusConfiguration);



            messagePublisher.Publish(new TestMessage());



            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }


    public class TestMessage
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
