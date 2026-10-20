using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Screenshots.Editor
{

    /// <summary>
    /// Provides utility methods for generating dynamic file names based on user-defined macro templates.
    /// </summary>
    internal static class FileNameProvider
    {

        /// <summary>
        /// Parses a format string and replaces internal macros with actual time, project, and resolution data.
        /// </summary>
        /// <param name="format">The template string containing macros (e.g., "{P}_{S}_{R}").</param>
        /// <param name="size">The resolution of the screenshot, used to replace the {R} macro.</param>
        /// <returns>A formatted string ready to be used as a file name, or "Default" if the format is empty.</returns>
        /// <remarks>
        /// Supported macros:
        /// <list type="bullet">
        /// <item><c>{Y}</c> - Year (yyyy)</item>
        /// <item><c>{M}</c> - Month (MM)</item>
        /// <item><c>{D}</c> - Day (dd)</item>
        /// <item><c>{h}</c> - Hour (HH)</item>
        /// <item><c>{m}</c> - Minute (mm)</item>
        /// <item><c>{s}</c> - Second (ss)</item>
        /// <item><c>{f}</c> - Milliseconds (fff)</item>
        /// <item><c>{S}</c> - Active Scene name</item>
        /// <item><c>{P}</c> - Project (Product) name</item>
        /// <item><c>{R}</c> - Resolution (Width x Height)</item>
        /// </list>
        /// </remarks>
        public static string Get(string format, Vector2Int size)
        {
            if (string.IsNullOrEmpty(format))
                return "Default";

            DateTime now = DateTime.Now;

            StringBuilder sb = new StringBuilder(format);

            sb.Replace("{Y}", now.ToString("yyyy"));
            sb.Replace("{M}", now.ToString("MM"));
            sb.Replace("{D}", now.ToString("dd"));
            sb.Replace("{h}", now.ToString("HH"));
            sb.Replace("{m}", now.ToString("mm"));
            sb.Replace("{s}", now.ToString("ss"));
            sb.Replace("{f}", now.ToString("fff"));

            sb.Replace("{S}", SceneManager.GetActiveScene().name);
            sb.Replace("{P}", UnityEngine.Application.productName);

            sb.Replace("{R}", $"{size.x}x{size.y}");

            return sb.ToString();
        }

    }

}
