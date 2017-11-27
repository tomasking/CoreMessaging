namespace Moonpig.Messaging.Consumer
{
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