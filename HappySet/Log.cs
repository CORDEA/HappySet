namespace HappySet
{
    public class Log
    {
        public string Id { get; }

        public string Message { get; }

        public Log(string id, string message)
        {
            Id = id;
            Message = message;
        }
    }
}