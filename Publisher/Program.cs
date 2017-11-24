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


            for (int i = 0; i < 10; i++)
            {
                messagePublisher.Publish(new TestMessage(i, "Name" + i));
            }


            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }


    public class TestMessage
    {
        public TestMessage(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
