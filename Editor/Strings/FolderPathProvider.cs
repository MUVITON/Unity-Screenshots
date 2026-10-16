using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace Screenshots.Editor
{

    public static class FolderPathProvider
    {

        public const string PackagePath = "Assets/Editor/Screenshots";

        private static Dictionary<Scene, string> _cache;

        public static string Get()
        {
            _cache ??= new Dictionary<Scene, string>();

            Scene scene = SceneManager.GetActiveScene();
            if (_cache.TryGetValue(scene, out string path))
                return path;

            path = $"{PackagePath}/{scene.name}";
            _cache[scene] = path;
            return path;
        }

    }

}
