using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class DefaultFolderPathController : FolderPathController
    {

        const string DefaultPath = "Screenshots";


        public DefaultFolderPathController(FolderField field, SerializedProperty property)
            : base(field, property)
        {
            Field.SecondButton.RegisterCallback<ClickEvent>(e => SetDefaultPath());
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

            Field.Input.SetValueWithoutNotify(value);
            Field.SecondButton.SetEnabled(value != DefaultPath);
        }

    }

}
