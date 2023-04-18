namespace SimpleLiveChat.Services
{
    public static class KeyExtractor
    {
        public static string? GetKey(string compoundKey) {
            return compoundKey.Split(":").ElementAtOrDefault(1);
        }

        public static string? GetEntity(string compoundKey) {
            return compoundKey.Split(":").ElementAtOrDefault(2);
        }
    }
}