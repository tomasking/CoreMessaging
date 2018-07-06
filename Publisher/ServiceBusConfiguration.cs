namespace Messaging.Publisher
{
	using System;
	using Messaging;
	using Microsoft.Extensions.Configuration;

	public class ServiceBusConfiguration : IServiceBusConfiguration
    {
        protected readonly IConfiguration Configuration;

        public ServiceBusConfiguration(IConfigurationBuilder configurationBuilder)
        {
            Configuration = configurationBuilder
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public string ConnectionString
        {
            get
            {
#if DEBUG
                var serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusPrimaryConnection");
                if (serviceBusConnectionString != null)
                {
                    return serviceBusConnectionString;
                }
 #endif
                
                return Configuration["ServiceBusConnectionString"];
            }
        }

        public string TopicName => Configuration["ServiceBusTopicName"];

        public TimeSpan MessageTimeToLive => TimeSpan.FromHours(int.Parse(Configuration["ServiceBusMessageTimeToLiveInHours"]));

        public TimeSpan RetryPolicyMinimumBackoff => TimeSpan.FromHours(int.Parse(Configuration["ServiceBusRetryPolicyMinimumBackoffInSeconds"]));

        public TimeSpan RetryPolicyMaximumBackoff => TimeSpan.FromHours(int.Parse(Configuration["ServiceBusRetryPolicyMaximumBackoffInSeconds"]));

        public int RetryPolicyMaximumRetryCount => int.Parse(Configuration["ServiceBusRetryPolicyMaximumRetryCount"]);
    }
}