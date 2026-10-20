using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Controls the toggle for automatically revealing the saved screenshot in the OS file browser.
    /// </summary>
    internal sealed class OpenAfterCaptureController : EditorPrefsSelectionController<Toggle>
    {

        /// <summary>
        /// Initializes the controller and binds it to the specific toggle element.
        /// </summary>
        /// <param name="toggle">The UI Toolkit toggle element.</param>
        public OpenAfterCaptureController(Toggle toggle) : base(toggle)
        {
            toggle.RegisterValueChangedCallback(e => SetState(e.newValue));
        }

        /// <summary>
        /// Applies the state directly to the toggle without triggering a value-changed event.
        /// </summary>
        /// <param name="toggle">The UI Toolkit toggle element.</param>
        /// <param name="isSelected">The state to apply.</param>
        protected override void ApplyState(Toggle toggle, bool isSelected)
        {
            toggle.SetValueWithoutNotify(isSelected);
        }

        /// <summary>
        /// Opens the provided path in the operating system's file browser if the toggle is enabled.
        /// </summary>
        /// <param name="path">The absolute path to the saved screenshot file.</param>
        public void OpenPath(string path) 
        {
            if (IsSelected)
                EditorUtility.RevealInFinder(path);
        }

    }

}
