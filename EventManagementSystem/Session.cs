using EventManagementSystem.Models;

namespace EventManagementSystem
{
    public static class Session
    {
        public static User CurrentUser { get; set; } = null!;
        public static bool IsLoggedIn => CurrentUser != null;
        public static bool IsAdmin => CurrentUser?.IsAdmin == true;
        public static void Logout() => CurrentUser = null!;
    }
}