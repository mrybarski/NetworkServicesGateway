namespace NetworkServicesGateway.Extensions
{
    public static class HttpSessionExtension
    {
        private const string SessionUserName = "_Name";
        public static string? GetUserName(this ISession session) => session.GetString(SessionUserName);
        public static void SetUserName(this ISession session, string name) => session.SetString(SessionUserName, name);
    }
}
