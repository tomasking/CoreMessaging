namespace Moonpig.Messaging
{
    using System;

    public interface IServiceBusConfiguration
    {
        string ConnectionString { get; }

        string TopicName { get; }

        TimeSpan MessageTimeToLive { get; }

        TimeSpan RetryPolicyMinimumBackoff { get; }

        TimeSpan RetryPolicyMaximumBackoff { get; }

        int RetryPolicyMaximumRetryCount { get; }
    }
}