using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Controls the UI for the base fallback directory used when auto-generating paths.
    /// </summary>
    internal sealed class DefaultFolderPathController : FolderPathController
    {

        /// <summary>
        /// The hardcoded fallback directory name used if the user clears the field.
        /// </summary>
        const string DefaultPath = "Screenshots";

        private readonly Button _resetBtn;


        /// <summary>
        /// Initializes the controller, binds the reset button, and synchronizes the initial state.
        /// </summary>
        /// <param name="container">The root visual element containing the default folder UI.</param>
        /// <param name="property">The serialized property representing the default folder path.</param>
        public DefaultFolderPathController(VisualElement container, SerializedProperty property)
            : base(container, property)
        {
            _resetBtn = container.Q<Button>("reset-btn");
            _resetBtn.RegisterCallback<ClickEvent>(e => SetDefaultPath());

            OnPropertyChanged(Property);
        }

        /// <summary>
        /// Reverts the serialized property to the base default path and applies the changes.
        /// </summary>
        private void SetDefaultPath()
        {
            Property.stringValue = DefaultPath;
            Property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Responds to changes in the tracked serialized property, ensuring it is never empty.
        /// Updates the text field and the interactable state of the reset button.
        /// </summary>
        /// <param name="property">The updated serialized property.</param>
        protected override void OnPropertyChanged(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.stringValue))
                SetDefaultPath();

            string value = property.stringValue;

            _resetBtn.SetEnabled(value != DefaultPath);

            Input.SetValueWithoutNotify(value);
        }

    }

}
