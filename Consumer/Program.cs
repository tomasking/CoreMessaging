namespace Messaging.Consumer
{
	using System;
	using System.Threading.Tasks;
	using Messaging;
	using Microsoft.Extensions.Configuration;

	class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var config = new ServiceBusConfiguration(new ConfigurationBuilder());

            IServiceBus serviceBus = new ServiceBus(config);

            serviceBus.Subscribe<TestMessage>(DoSomething);

            Console.ReadKey();

            serviceBus.Dispose();
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