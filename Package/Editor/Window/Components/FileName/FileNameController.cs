using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Controls the UI logic for configuring the screenshot file name and its extension type.
    /// </summary>
    internal sealed class FileNameController
    {

        // Updated to include the {R} macro so users discover the resolution feature immediately.
        const string defaultName = "{P}_{S}_{Y}{M}{D}";

        private readonly TextField _field;
        private readonly Label _preview;
        private readonly EnumField _fileType;
        private readonly Button _resetBtn;

        private Vector2Int _size;

        /// <summary>
        /// Gets the generated file name with all macros replaced by current values.
        /// Evaluates immediately upon access to ensure time-based macros are accurate at the exact moment of capture.
        /// </summary>
        public string Value
        {
            get
            {
                UpdateValue();
                return _value;
            }
        }
        private string _value;

        /// <summary>
        /// Gets the selected file extension type (e.g., png, jpg, tga).
        /// </summary>
        public ScreenshotType Type => (ScreenshotType)_fileType.value;


        /// <summary>
        /// Initializes the controller and binds the visual elements.
        /// </summary>
        /// <param name="container">The root UI element containing the file name controls.</param>
        /// <param name="size">The initial resolution size used for the {R} macro.</param>
        public FileNameController(VisualElement container, Vector2Int size)
        {
            _size = size;

            _preview  = container.Q<Label>("file-name__preview");
            _fileType = container.Q<EnumField>("file-name__type");

            _field = container.Q<TextField>("file-name__field");
            _field.RegisterValueChangedCallback(e => UpdateValue());
            _field.RegisterCallback<FocusOutEvent>(e => OnFocusOut());

            _resetBtn = container.Q<Button>("file-name__reset-btn");
            _resetBtn.RegisterCallback<ClickEvent>(e => ResetName());

            OnFocusOut();
            UpdateValue();
        }

        /// <summary>
        /// Recalculates the parsed file name based on the current input and updates the preview label.
        /// </summary>
        private void UpdateValue()
        {
            string input = string.IsNullOrEmpty(_field.value) ? defaultName : _field.value;

            _value = FileNameProvider.Get(input, _size);
            _preview.text = _value;

            _resetBtn.SetEnabled(_field.value != defaultName);
        }

        /// <summary>
        /// Handles the focus out event to ensure the field is never left completely empty.
        /// </summary>
        private void OnFocusOut()
        {
            if (string.IsNullOrEmpty(_field.value))
                ResetName();
        }

        /// <summary>
        /// Resets the input field to the default macro template.
        /// </summary>
        private void ResetName()
        {
            _field.value = defaultName;
        }

        /// <summary>
        /// Updates the stored resolution size and refreshes the preview.
        /// </summary>
        /// <param name="size">The new dimensions of the screenshot.</param>
        public void UpdateSize(Vector2Int size)
        {
            _size = size;
            UpdateValue();
        }

    }

}
