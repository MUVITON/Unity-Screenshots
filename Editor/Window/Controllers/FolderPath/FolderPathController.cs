using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public abstract class FolderPathController
    {

        #region State

        protected FolderField Field { get; }
        protected SerializedProperty Property { get; }   

        public string Path => Field.Input.value;

        #endregion


        #region Initialization

        public FolderPathController(FolderField field, SerializedProperty property)
        {
            Property = property;
            Field    = field;

            Field.Input.RegisterCallback<ClickEvent>(e => { if (e.clickCount == 2) OpenFolder(); });
            Field.Input.TrackPropertyValue(Property, OnPropertyChanged);
            Field.FirstButton.RegisterCallback<ClickEvent>(e => SelectFolder());
        }

        #endregion


        #region Event Handlers

        protected abstract void OnPropertyChanged(SerializedProperty property);

        #endregion


        #region Navigation

        private void SelectFolder()
        {
            string folderPath = EditorUtility.OpenFolderPanel("Choose folder", Path, "");

            if (string.IsNullOrEmpty(folderPath))
                return;

            string normalizePath = ProjectPathProvider.GetForProject(folderPath);

            Property.stringValue = string.IsNullOrEmpty(normalizePath) ? folderPath : normalizePath;
            Property.serializedObject.ApplyModifiedProperties();
        }

        private void OpenFolder()
        {
            EditorUtility.RevealInFinder(Path + System.IO.Path.DirectorySeparatorChar);
        }

        #endregion

    }

}
