using System;

namespace Publisher
{
    using Microsoft.Extensions.Configuration;
    using Moonpig.Messaging;
    using Moonpig.Messaging.Consumer;

    class Program
    {
        static void Main(string[] args)
        {
            var config = new ServiceBusConfiguration(new ConfigurationBuilder());

            IServiceBus serviceBus = new ServiceBus(config);

            for (int i = 0; i < 10; i++)
            {
                serviceBus.Publish(new TestMessage(i, "Name" + i));
            }
            
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
