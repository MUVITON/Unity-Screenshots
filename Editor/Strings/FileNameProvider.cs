using System;
using System.Text;
using UnityEngine.SceneManagement;


namespace Screenshots.Editor
{

    public static class FileNameProvider
    {
        public static string Get(string format)
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

            return sb.ToString();
        }

    }

}
