namespace BattleNotifications.Api.Extensions
{
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    public static class UserExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }
    }
}