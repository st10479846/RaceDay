namespace RaceDay.Api.Filters;

public static class SessionExtensions
{
    public const string UserIdKey = "UserId";
    public const string RoleKey = "Role";
    public const string NameKey = "FullName";

    public static int? GetUserId(this ISession session) =>
        session.GetInt32(UserIdKey);

    public static string? GetRole(this ISession session) =>
        session.GetString(RoleKey);

    public static string? GetFullName(this ISession session) =>
        session.GetString(NameKey);

    public static bool IsLoggedIn(this ISession session) =>
        session.GetInt32(UserIdKey) != null;

    public static void SetUser(this ISession session, int userId, string role, string fullName)
    {
        session.SetInt32(UserIdKey, userId);
        session.SetString(RoleKey, role);
        session.SetString(NameKey, fullName);
    }

    public static void ClearUser(this ISession session) =>
        session.Clear();
}