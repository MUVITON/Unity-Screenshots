using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    internal sealed class IconButtonSelectionController
    {

        public const string SelectedClassName = "icon-btn--selected";

        private readonly string _saveKey;
        private readonly Button _btn;

        public bool IsSelected { get; private set; }


        public IconButtonSelectionController(Button btn)
        {
            _btn = btn;

            string id = !string.IsNullOrEmpty(btn.viewDataKey) ? btn.viewDataKey : btn.name;
            _saveKey = $"Screenshots_UI_State_{id}";

            IsSelected = EditorPrefs.GetBool(_saveKey, false);
            ApplyState(_btn, IsSelected);
        }

        public static void ApplyState(Button btn, bool isSelected)
        {
            if (isSelected)
                btn.AddToClassList(SelectedClassName);
            else
                btn.RemoveFromClassList(SelectedClassName);
        }

        public void SetState(bool value)
        {
            if (IsSelected == value)
                return;

            IsSelected = value;

            EditorPrefs.SetBool(_saveKey, IsSelected);
            ApplyState(_btn, IsSelected);
        }

    }

}
