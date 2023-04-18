namespace SimpleLiveChat.Models.CustomExceptions
{
    public class ConsumerException : Exception
    {
        public ConsumerException(string message) : base(message) { }
    }
}