using SimpleLiveChat.Interfaces;

namespace SimpleLiveChat.Models.CustomExceptions
{
    public class KeyNotFoundException<T, K> : Exception
    where T : notnull
    {
        public KeyNotFoundException(T key) : base($"{nameof(K)} object with key {key.ToString()} not found") { }
    }
}