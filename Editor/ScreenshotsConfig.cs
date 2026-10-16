using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Screenshots.Editor
{

    public class ScreenshotsConfig: ScriptableObject
    {

        public static readonly Vector2Int DefaultLandscapeSize = new Vector2Int(2556, 1179);
        public static readonly Vector2Int DefaultPortraitSize = new Vector2Int(1179, 2556);

        private static ScreenshotsConfig _instance;
        public static ScreenshotsConfig Instance => _instance ??= GetInstance();

        [field: SerializeField, Tooltip("")]
        public Vector2Int LandscapeSize { get; set; } = DefaultLandscapeSize;

        [field: SerializeField, Tooltip("")]
        public Vector2Int PortraitSize { get; set; } = DefaultPortraitSize;

        [field: SerializeField, Tooltip("")]
        public ScreenshotOrientation Orientation { get; set; }

        [field: SerializeField, Tooltip("")]
        public string FolderPath { get; set; }


        public static ScreenshotsConfig GetInstance()
        {
            const string assetPath = FolderPathProvider.PackagePath + "/ScreenshotsConfig.asset";

            var config = AssetDatabase.LoadAssetAtPath<ScreenshotsConfig>(assetPath);
            if (config != null)
            {
                return config;
            }

            if (!Directory.Exists(FolderPathProvider.PackagePath))
                Directory.CreateDirectory(FolderPathProvider.PackagePath);

            config = CreateInstance<ScreenshotsConfig>();
            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();

            return config;
        }

        public static Vector2Int GetSize()
        {
            return Instance.Orientation switch
            {
                ScreenshotOrientation.Landscape => Instance.LandscapeSize,
                ScreenshotOrientation.Portrait => Instance.PortraitSize,
                _ => throw new ArgumentOutOfRangeException(nameof(Instance.Orientation))
            };
        }

    }

}
