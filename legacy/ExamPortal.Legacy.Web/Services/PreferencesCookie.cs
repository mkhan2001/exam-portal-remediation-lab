using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ExamPortal.Legacy.Web.Models;

namespace ExamPortal.Legacy.Web.Services
{
    /// <summary>Stores display preferences in a cookie so they survive between visits.</summary>
    public static class PreferencesCookie
    {
        public const string CookieName = "ep_prefs";

        public static string Serialize(UserPreferences preferences)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, preferences);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static UserPreferences Deserialize(string cookieValue)
        {
            if (string.IsNullOrEmpty(cookieValue))
            {
                return new UserPreferences { Theme = "light", PageSize = 25 };
            }

            using (var stream = new MemoryStream(Convert.FromBase64String(cookieValue)))
            {
                return (UserPreferences)new BinaryFormatter().Deserialize(stream);
            }
        }
    }
}
