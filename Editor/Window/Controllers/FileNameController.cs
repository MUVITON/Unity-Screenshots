using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class FileNameController
    {

        const string defaultName = "{P}_{S}_{Y}{M}{D}";

        private readonly TextField _field;
        private readonly Label _preview;
        private readonly EnumField _fileType;

        private string _value;
        public string Value
        {
            get
            {
                UpdateValue();
                return _value;
            }
        }

        public ScreenshotType Type => (ScreenshotType)_fileType.value;


        public FileNameController(VisualElement container)
        {
            _field = container.Q<TextField>("file-name__field");
            _preview = container.Q<Label>("file-name__preview");
            _fileType = container.Q<EnumField>("file-name__type");

            _field.RegisterValueChangedCallback(e => UpdateValue());
            _field.RegisterCallback<FocusOutEvent>(e => OnFocusOut());

            var resetBtn = container.Q<Button>("file-name__reset-btn");
            resetBtn.RegisterCallback<ClickEvent>(e => ResetName());

            OnFocusOut();
            UpdateValue();
        }

        private void UpdateValue()
        {
            string input = string.IsNullOrEmpty(_field.value) ? defaultName : _field.value;

            _value = FileNameProvider.Get(input);
            _preview.text = _value;
        }

        private void OnFocusOut()
        {
            if (string.IsNullOrEmpty(_field.value))
                ResetName();
        }

        private void ResetName()
        {
            _field.value = defaultName;
        }

    }

}
