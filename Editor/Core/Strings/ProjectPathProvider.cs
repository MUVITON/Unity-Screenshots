using System.IO;
using UnityEngine;


namespace Screenshots.Editor
{

    internal static class ProjectPathProvider
    {

        private static string _projectRoot;
        public static string ProjectRoot => _projectRoot ??= Application.dataPath.Substring(0, Application.dataPath.Length - 7);


        private static string GetNormalize(string path) => path.Replace('\\', '/');

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
