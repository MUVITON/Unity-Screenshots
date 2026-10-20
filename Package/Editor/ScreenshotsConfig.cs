using System;
using UnityEditor;
using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// Stores the user's persistent preferences and state for the Screenshots window.
    /// Saved locally in the UserSettings folder to avoid version control conflicts.
    /// </summary>
    [FilePath("UserSettings/ScreenshotsConfig.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class ScreenshotsConfig: ScriptableSingleton<ScreenshotsConfig>
    {

        /// <summary>
        /// The currently selected orientation mode (e.g., Landscape, Portrait, or Custom).
        /// </summary>
        [field: SerializeField, Tooltip("The active orientation mode for the screenshot.")]
        public ScreenshotOrientation Orientation { get; set; }

        /// <summary>
        /// The currently selected landscape resolution preset.
        /// </summary>
        [field: SerializeField, Tooltip("The selected landscape resolution preset.")]
        public LandscapeResolution LandscapeResolution { get; set; }

        /// <summary>
        /// The currently selected portrait resolution preset.
        /// </summary>
        [field: SerializeField, Tooltip("The selected portrait resolution preset.")]
        public PortraitResolution PortraitResolution { get; set; }

        /// <summary>
        /// The custom resolution dimensions defined by the user.
        /// Changed to Vector2Int since pixel dimensions cannot be fractional.
        /// </summary>
        [field: SerializeField, Tooltip("Custom width and height in pixels.")]
        public Vector2 CustomResolution { get; set; } 
            = LandscapeResolutionProvider.GetSize(LandscapeResolution.FullHD);

        /// <summary>
        /// The active directory path where screenshots will be saved.
        /// </summary>
        [field: SerializeField, Tooltip("The target folder path for saving screenshots.")]
        public string FolderPath { get; set; }

        /// <summary>
        /// The fallback or default directory path for saving screenshots.
        /// </summary>
        [field: SerializeField, Tooltip("The default fallback folder path.")]
        public string DefaultFolderPath { get; set; }

        /// <summary>
        /// The current macro template used for generating file names.
        /// </summary>
        [field: SerializeField, Tooltip("The macro template used to generate the file name.")]
        public string FileName { get; set; }

        /// <summary>
        /// The selected image format for the captured screenshots.
        /// </summary>
        [field: SerializeField, Tooltip("The output file format for the screenshot.")]
        public ScreenshotType FileType { get; set; }


        /// <summary>
        /// Ensures that the configuration is saved to disk when the ScriptableObject is unloaded or Unity closes.
        /// </summary>
        private void OnDisable()
        {
            Save(true);
        }

    }

}
