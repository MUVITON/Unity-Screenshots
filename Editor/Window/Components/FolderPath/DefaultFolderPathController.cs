using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class DefaultFolderPathController : FolderPathController
    {

        const string DefaultPath = "Screenshots";

        private readonly Button _resetBtn;


        public DefaultFolderPathController(VisualElement container, SerializedProperty property)
            : base(container, property)
        {
            _resetBtn = container.Q<Button>("reset-btn");
            _resetBtn.RegisterCallback<ClickEvent>(e => SetDefaultPath());

            OnPropertyChanged(Property);
        }

        private void SetDefaultPath()
        {
            Property.stringValue = DefaultPath;
            Property.serializedObject.ApplyModifiedProperties();
        }

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
