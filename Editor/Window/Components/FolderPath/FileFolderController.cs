using System;
using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    internal sealed class FileFolderController : IDisposable
    {

        private readonly DefaultFolderPathController _defaultFolderPath;
        private readonly ActiveFolderPathController _folderPath;

        public string Path => _folderPath.Path;


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

        public void Dispose() 
        {
            _folderPath.Dispose();
        }

    }

}
