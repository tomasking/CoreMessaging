﻿namespace Moonpig.Messaging
{
    public class TopicDefinition
    {
        public TopicDefinition(string topicName)
        {
            TopicName = topicName;
        }

        public string TopicName { get; private set; }
    }
}