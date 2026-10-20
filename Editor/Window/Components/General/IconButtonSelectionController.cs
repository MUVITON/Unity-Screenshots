using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Controls the selection state and visual styling of an icon-based button.
    /// Persists the toggle state across domain reloads and editor sessions using EditorPrefs.
    /// </summary>
    internal sealed class IconButtonSelectionController : EditorPrefsSelectionController<Button>
    {

        /// <summary>
        /// The CSS class name applied to the button when it is in the active or selected state.
        /// </summary>
        public const string SelectedClassName = "icon-btn--selected";


        /// <summary>
        /// Initializes the controller, generates a unique save key based on the button's ID, 
        /// and restores the previous state from EditorPrefs.
        /// </summary>
        /// <param name="btn">The UI Toolkit button element to control.</param>
        public IconButtonSelectionController(Button btn) : base(btn) { }

        /// <summary>
        /// A static helper method to visually update any button's CSS classes without altering EditorPrefs.
        /// </summary>
        /// <param name="btn">The button to update.</param>
        /// <param name="isSelected">True to add the selected styling, false to remove it.</param>
        public static void ApplyClass(VisualElement element, bool isSelected)
        {
            if (isSelected)
                element.AddToClassList(SelectedClassName);
            else
                element.RemoveFromClassList(SelectedClassName);
        }

        /// <inheritdoc />
        protected override void ApplyState(Button element, bool isSelected)
        {
            ApplyClass(element, isSelected);
        }

    }

}
