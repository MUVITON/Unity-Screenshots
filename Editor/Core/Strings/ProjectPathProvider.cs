using System.IO;
using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// Provides utility methods for normalizing file paths and calculating relative paths 
    /// within the Unity project environment.
    /// </summary>
    internal static class ProjectPathProvider
    {

        /// <summary>
        /// The absolute path to the root directory of the current Unity project (one level above the Assets folder).
        /// </summary>
        public static string ProjectRoot => _projectRoot ??= Application.dataPath.Substring(0, Application.dataPath.Length - 7);
        private static string _projectRoot;


        /// <summary>
        /// Normalizes backslashes to forward slashes for cross-platform compatibility.
        /// </summary>
        private static string GetNormalize(string path) => path.Replace('\\', '/');

        /// <summary>
        /// Converts an absolute or arbitrary path into a relative path starting from the Project Root.
        /// </summary>
        /// <param name="path">The path to process.</param>
        /// <returns>A path relative to the Project Root, or the normalized path if it's already relative. Returns <c>null</c> if the path is outside the project hierarchy.</returns>
        public static string GetForProject(string path)
        {
            string normalizedPath = GetNormalize(path);
            string relativePath = null;

            if (normalizedPath.StartsWith(ProjectRoot))
            {
                if (normalizedPath.Length > ProjectRoot.Length)
                    relativePath = normalizedPath.Substring(ProjectRoot.Length + 1);
                else
                    relativePath = string.Empty;
            }
            else if (!Path.IsPathRooted(normalizedPath))
            {
                relativePath = normalizedPath;
            }

            return relativePath;
        }

        /// <summary>
        /// Converts an absolute or arbitrary path into a relative path starting with the "Assets" folder.
        /// Useful for AssetDatabase operations.
        /// </summary>
        /// <param name="path">The path to process.</param>
        /// <returns>A path formatted as "Assets/...", or <c>null</c> if the path is outside the Assets folder.</returns>
        public static string GetForAsset(string path)
        {
            string normalizedPath = GetNormalize(path);
            string relativePath = null;

            if (normalizedPath.StartsWith(Application.dataPath))
            {
                relativePath = "Assets" + normalizedPath.Substring(Application.dataPath.Length);
            }
            else if (normalizedPath.StartsWith("Assets/"))
            {
                relativePath = normalizedPath;
            }

            return relativePath;
        }

    }

}
