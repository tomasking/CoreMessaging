namespace Consumer
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Configuration;
    using Moonpig.Messaging.Subscribing;

    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var config = new ServiceBusConfiguration(new ConfigurationBuilder());

            var subscriptionClient = new SubscriptionClient(config.ConnectionString, config.TopicName, "test");
            
            ServiceBus serviceBus =new ServiceBus(subscriptionClient);

           serviceBus.Subscribe<TestMessage>(DoSomething);

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
        }

        private static void DoSomething(TestMessage obj)
        {
            Console.Write($"Message Received Id {0}, Name {1}", obj.Id, obj.Name);
        }
    }

    public class TestMessage
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}