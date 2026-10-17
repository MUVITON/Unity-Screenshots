using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class FileNameController
    {

        const string defaultName = "{P}_{S}_{Y}{M}{D}";

        private readonly TextField _field;

        public string Value => FileNameProvider.Get(_field.value);

        public FileNameController(TextField field, Button resetBtn)
        {
            _field = field;

            field.RegisterCallback<FocusOutEvent>(e => OnFocusOut());
            resetBtn.RegisterCallback<ClickEvent>(e => ResetName());

            OnFocusOut();
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
