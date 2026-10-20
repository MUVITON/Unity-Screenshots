using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// An abstract base controller for managing folder path selection via the UI.
    /// Handles user interactions like opening the folder dialog and revealing the directory in the OS file explorer.
    /// </summary>
    internal abstract class FolderPathController
    {

        #region State

        /// <summary>
        /// The text field displaying the current folder path.
        /// </summary>
        protected TextField Input { get; }

        /// <summary>
        /// The serialized property linked to the folder path setting.
        /// </summary>
        protected SerializedProperty Property { get; }

        /// <summary>
        /// Gets the current path string directly from the input field's value.
        /// </summary>
        public string Path => Input.value;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes the controller, binds the UI elements, and sets up event listeners.
        /// </summary>
        /// <param name="container">The root visual element containing the input field and selection button.</param>
        /// <param name="property">The serialized property to track and update.</param>
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

        /// <summary>
        /// Invoked automatically by the UI Toolkit when the tracked serialized property changes.
        /// Must be implemented by derived classes to handle specific logic.
        /// </summary>
        /// <param name="property">The updated serialized property.</param>
        protected abstract void OnPropertyChanged(SerializedProperty property);

        #endregion


        #region Navigation

        /// <summary>
        /// Opens the native system folder selection dialog and updates the property with the chosen path.
        /// Automatically normalizes the path to be relative to the project root if applicable.
        /// </summary>
        private void SelectFolder()
        {
            string folderPath = EditorUtility.OpenFolderPanel("Choose folder", Path, "");

            if (string.IsNullOrEmpty(folderPath))
                return;

            string normalizePath = ProjectPathProvider.GetForProject(folderPath);

            Property.stringValue = string.IsNullOrEmpty(normalizePath) ? folderPath : normalizePath;
            Property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Reveals the currently assigned folder in the operating system's native file explorer (Finder/Explorer).
        /// </summary>
        private void OpenFolder()
        {
            EditorUtility.RevealInFinder(Path + System.IO.Path.DirectorySeparatorChar);
        }

        #endregion

    }

}
