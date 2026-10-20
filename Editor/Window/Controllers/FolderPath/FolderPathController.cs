using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public abstract class FolderPathController
    {

        #region State

        protected TextField Input { get; }
        protected SerializedProperty Property { get; }   

        public string Path => Input.value;

        #endregion


        #region Initialization

        public FolderPathController(VisualElement container, SerializedProperty property)
        {
            Property = property;

            Input = container.Q<TextField>();
            Input.RegisterCallback<ClickEvent>(e => { if (e.clickCount == 2) OpenFolder(); });
            Input.TrackPropertyValue(Property, OnPropertyChanged);
            
            var selectBtn = container.Q<Button>("select-btn");
            selectBtn.RegisterCallback<ClickEvent>(e => SelectFolder());
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
