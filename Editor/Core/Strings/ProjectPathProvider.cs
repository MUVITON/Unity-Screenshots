using UnityEngine;


namespace Screenshots.Editor
{

    internal static class ProjectPathProvider
    {

        public static string Get(string path)
        {
            string normalizedPath = path.Replace('\\', '/');
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
