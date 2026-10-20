using System;
using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// A facade controller that manages both the active and default folder path UI components.
    /// Encapsulates the complexity of the folder hierarchy from the main window.
    /// </summary>
    internal sealed class FileFolderController : IDisposable
    {

        private readonly DefaultFolderPathController _defaultFolderPath;
        private readonly ActiveFolderPathController _folderPath;

        /// <summary>
        /// Gets the final, resolved folder path to be used for saving screenshots.
        /// </summary>
        public string Path => _folderPath.Path;


        /// <summary>
        /// Initializes the aggregate folder controller by creating and binding its sub-controllers.
        /// </summary>
        /// <param name="container">The main visual element containing all folder-related UI.</param>
        /// <param name="activeProperty">The serialized property for the active/custom path.</param>
        /// <param name="defaultProperty">The serialized property for the base default path.</param>
        public FileFolderController(
            VisualElement container, 
            SerializedProperty activeProperty, 
            SerializedProperty defaultProperty
        )
        {
            _defaultFolderPath = new DefaultFolderPathController(
                container.Q<VisualElement>("default-folder-path"),
                defaultProperty
            );

            _folderPath = new ActiveFolderPathController(
                container.Q<VisualElement>("active-folder-path"),
                activeProperty,
                defaultProperty
            );
        }

        /// <summary>
        /// Cleans up resources and unregisters events in the active folder controller.
        /// </summary>
        public void Dispose() 
        {
            _folderPath.Dispose();
        }

    }

}
