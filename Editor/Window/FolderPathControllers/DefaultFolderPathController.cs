using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class DefaultFolderPathController : FolderPathController
    {

        public DefaultFolderPathController(FolderField field, SerializedProperty property)
            : base(field, property)
        {
            Field.SecondButton.RegisterCallback<ClickEvent>(e => SetDefaultPath());
            OnPropertyChanged(Property);
        }

        private void SetDefaultPath()
        {
            Property.stringValue = FolderPathProvider.PackagePath;
            Property.serializedObject.ApplyModifiedProperties();
        }

        protected override void OnPropertyChanged(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.stringValue))
                SetDefaultPath();
            
            Field.Input.SetValueWithoutNotify(property.stringValue);
        }

    }

}
