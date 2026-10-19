using UnityEngine;


namespace Screenshots.Editor
{

    internal static class ScreenshotsLogger
    {

        const string Prefix = "<b>[Screenshots]</b>";


        public static void Log(string message)
        {
            Debug.Log($"{Prefix}: {message}");
        }

        public static void Warn(string message)
        {
            Debug.LogWarning($"{Prefix}: {message}");
        }

        public static void Error(string message)
        {
            Debug.LogError($"{Prefix} {message}");
        }

    }

}
