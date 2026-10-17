using System;
using UnityEditor;
using UnityEngine;


namespace Screenshots.Editor
{

    [FilePath("UserSettings/ScreenshotsConfig.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class ScreenshotsConfig: ScriptableSingleton<ScreenshotsConfig>
    {

        public static readonly Vector2Int DefaultLandscapeSize = new Vector2Int(2556, 1179);
        public static readonly Vector2Int DefaultPortraitSize  = new Vector2Int(1179, 2556);


        [field: SerializeField, Tooltip("")]
        public Vector2Int LandscapeSize { get; set; } = DefaultLandscapeSize;

        [field: SerializeField, Tooltip("")]
        public Vector2Int PortraitSize { get; set; } = DefaultPortraitSize;

        [field: SerializeField, Tooltip("")]
        public ScreenshotOrientation Orientation { get; set; }

        [field: SerializeField, Tooltip("")]
        public string FolderPath { get; set; }

        [field: SerializeField, Tooltip("")]
        public string DefaultFolderPath { get; set; }


        public static Vector2Int GetSize()
        {
            return instance.Orientation switch
            {
                ScreenshotOrientation.Landscape => instance.LandscapeSize,
                ScreenshotOrientation.Portrait  => instance.PortraitSize,
                _ => throw new ArgumentOutOfRangeException(nameof(instance.Orientation))
            };
        }

        private void OnDisable()
        {
            Save(true);
        }

    }

}
