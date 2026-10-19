using System;
using UnityEditor;
using UnityEngine;


namespace Screenshots.Editor
{

    [FilePath("UserSettings/ScreenshotsConfig.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class ScreenshotsConfig: ScriptableSingleton<ScreenshotsConfig>
    {

        [field: SerializeField, Tooltip("")]
        public ScreenshotOrientation Orientation { get; set; }

        [field: SerializeField, Tooltip("")]
        public LandscapeResolution LandscapeResolution { get; set; }

        [field: SerializeField, Tooltip("")]
        public PortraitResolution PortraitResolution { get; set; }

        [field: SerializeField, Tooltip("")]
        public Vector2 CustomResolution { get; set; } 
            = LandscapeResolutionProvider.GetSize(LandscapeResolution.FullHD);

        [field: SerializeField, Tooltip("")]
        public string FolderPath { get; set; }

        [field: SerializeField, Tooltip("")]
        public string DefaultFolderPath { get; set; }

        [field: SerializeField, Tooltip("")]
        public string FileName { get; set; }

        [field: SerializeField, Tooltip("")]
        public ScreenshotType FileType { get; set; }


        private void OnDisable()
        {
            Save(true);
        }

    }

}
