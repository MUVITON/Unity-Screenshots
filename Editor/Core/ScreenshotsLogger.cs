using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// A centralized logging utility for the Screenshots tool, prefixing all console messages with a rich-text tag.
    /// </summary>
    internal static class ScreenshotsLogger
    {

        const string Prefix = "<b>[Screenshots]</b>";


        /// <summary>
        /// Logs a standard informational message to the Unity Console.
        /// </summary>
        /// <param name="message">The text to display.</param>
        public static void Log(string message)
        {
            Debug.Log($"{Prefix}: {message}");
        }

        /// <summary>
        /// Logs a warning message to the Unity Console.
        /// </summary>
        /// <param name="message">The warning text to display.</param>
        public static void Warn(string message)
        {
            Debug.LogWarning($"{Prefix}: {message}");
        }

        /// <summary>
        /// Logs an error message to the Unity Console.
        /// </summary>
        /// <param name="message">The error text to display.</param>
        public static void Error(string message)
        {
            Debug.LogError($"{Prefix}: {message}");
        }

    }

}
