// ─────────────────────────────────────────────────────────────
// AmazeCare.MVC/Helpers/SessionHelper.cs
//
// Manages JWT token storage in Session
// Patient/Doctor/Admin never sees the token
// Token is stored server-side in Session
// ─────────────────────────────────────────────────────────────

namespace AmazeCare.MVC.Helpers
{
    public static class SessionHelper
    {
        // Session Keys
        private const string TOKEN_KEY = "JwtToken";
        private const string ROLE_KEY = "UserRole";
        private const string USERID_KEY = "UserId";
        private const string USERNAME_KEY = "UserName";

        // ── SAVE after login ──────────────────────────────
        public static void SetUserSession(
            ISession session,
            string token,
            string role,
            int userId,
            string userName)
        {
            session.SetString(TOKEN_KEY, token);
            session.SetString(ROLE_KEY, role);
            session.SetString(USERID_KEY, userId.ToString());
            session.SetString(USERNAME_KEY, userName);
        }

        // ── GET Token ─────────────────────────────────────
        public static string? GetToken(ISession session) =>
            session.GetString(TOKEN_KEY);

        // ── GET Role ──────────────────────────────────────
        public static string? GetRole(ISession session) =>
            session.GetString(ROLE_KEY);

        // ── GET UserId ────────────────────────────────────
        public static int GetUserId(ISession session)
        {
            var id = session.GetString(USERID_KEY);
            return int.TryParse(id, out int userId) ? userId : 0;
        }

        // ── GET UserName ──────────────────────────────────
        public static string? GetUserName(ISession session) =>
            session.GetString(USERNAME_KEY);

        // ── CHECK if logged in ────────────────────────────
        public static bool IsLoggedIn(ISession session) =>
            !string.IsNullOrEmpty(session.GetString(TOKEN_KEY));

        // ── CHECK Role ────────────────────────────────────
        public static bool IsPatient(ISession session) =>
            session.GetString(ROLE_KEY) == "Patient";

        public static bool IsDoctor(ISession session) =>
            session.GetString(ROLE_KEY) == "Doctor";

        public static bool IsAdmin(ISession session) =>
            session.GetString(ROLE_KEY) == "Admin";

        // ── CLEAR on logout ───────────────────────────────
        public static void ClearSession(ISession session) =>
            session.Clear();
    }
}
