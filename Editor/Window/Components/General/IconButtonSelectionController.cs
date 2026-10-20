using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Controls the selection state and visual styling of an icon-based button.
    /// Persists the toggle state across domain reloads and editor sessions using EditorPrefs.
    /// </summary>
    internal sealed class IconButtonSelectionController
    {

        /// <summary>
        /// The CSS class name applied to the button when it is in the active or selected state.
        /// </summary>
        public const string SelectedClassName = "icon-btn--selected";

        private readonly string _saveKey;
        private readonly Button _btn;

        /// <summary>
        /// Gets the current logical selection state of the button.
        /// </summary>
        public bool IsSelected { get; private set; }


        /// <summary>
        /// Initializes the controller, generates a unique save key based on the button's ID, 
        /// and restores the previous state from EditorPrefs.
        /// </summary>
        /// <param name="btn">The UI Toolkit button element to control.</param>
        public IconButtonSelectionController(Button btn)
        {
            _btn = btn;

            string id = !string.IsNullOrEmpty(btn.viewDataKey) ? btn.viewDataKey : btn.name;
            _saveKey = $"Screenshots_UI_State_{id}";

            IsSelected = EditorPrefs.GetBool(_saveKey, false);
            ApplyState(_btn, IsSelected);
        }

        /// <summary>
        /// A static helper method to visually update any button's CSS classes without altering EditorPrefs.
        /// </summary>
        /// <param name="btn">The button to update.</param>
        /// <param name="isSelected">True to add the selected styling, false to remove it.</param>
        public static void ApplyState(Button btn, bool isSelected)
        {
            if (isSelected)
                btn.AddToClassList(SelectedClassName);
            else
                btn.RemoveFromClassList(SelectedClassName);
        }

        /// <summary>
        /// Updates the logical state, saves it to EditorPrefs, and refreshes the button's visual styling.
        /// </summary>
        /// <param name="value">The new selection state to apply.</param>
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
