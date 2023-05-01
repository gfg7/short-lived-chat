namespace SimpleLiveChat.Services
{
    public static class KeyService
    {
        public static string? GetKeyPart(string compoundKey, int index = 0) {
            return compoundKey.Split(":").ElementAtOrDefault(index);
        }

        public static string? CreateKey(params string [] keyParts) {
            return string.Join(":", keyParts);
        }
    }
}