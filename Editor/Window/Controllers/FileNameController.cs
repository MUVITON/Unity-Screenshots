using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class FileNameController
    {

        const string defaultName = "{P}_{S}_{Y}{M}{D}";

        private readonly TextField _field;
        private readonly Label _preview;

        private string _value;
        public string Value
        {
            get
            {
                UpdateValue();
                return _value;
            }
        }

        public FileNameController(TextField field, Label preview, Button resetBtn)
        {
            _field = field;
            _preview = preview;

            field.RegisterValueChangedCallback(e => UpdateValue());
            field.RegisterCallback<FocusOutEvent>(e => OnFocusOut());
            resetBtn.RegisterCallback<ClickEvent>(e => ResetName());

            OnFocusOut();
            UpdateValue();
        }

        private void UpdateValue()
        {
            string input = string.IsNullOrEmpty(_field.value) ? defaultName : _field.value;

            _value = FileNameProvider.Get(input);
            _preview.text = FileNameProvider.Get(_value);
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
