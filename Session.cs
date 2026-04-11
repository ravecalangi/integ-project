namespace Program
{
    public static class Session
    {
        public static User CurrentUser { get; set; }
        public static bool IsAdmin => CurrentUser?.IsAdmin == true;
        public static void Clear() => CurrentUser = null;
    }
}