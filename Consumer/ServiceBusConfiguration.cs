namespace Consumer
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Moonpig.Messaging;

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
                var serviceBusConnectionString = Environment.GetEnvironmentVariable("MNP-Skywalker.ESB.PrimaryConnection");
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