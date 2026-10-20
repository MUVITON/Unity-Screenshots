using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// An abstract controller that manages the selection state and visual styling of a specific UI Toolkit element.
    /// Persists the state across domain reloads and editor sessions using EditorPrefs.
    /// </summary>
    /// <typeparam name="TElement">The specific type of VisualElement being controlled.</typeparam>
    internal abstract class EditorPrefsSelectionController<TElement>
        where TElement : VisualElement
    {

        private readonly string _saveKey;
        private readonly TElement _element;

        /// <summary>
        /// Gets the current logical selection state of the element.
        /// </summary>
        public bool IsSelected { get; private set; }


        /// <summary>
        /// Initializes the controller, generates a unique save key based on the element's ID, 
        /// and restores the previous state from EditorPrefs.
        /// </summary>
        /// <param name="element">The UI Toolkit visual element to control.</param>
        protected EditorPrefsSelectionController(TElement element)
        {
            _element = element;

            // Fallback to name if viewDataKey is not set, ensuring a unique key.
            string id = !string.IsNullOrEmpty(_element.viewDataKey) ? _element.viewDataKey : _element.name;
            _saveKey = $"Screenshots_UI_State_{id}";

            IsSelected = EditorPrefs.GetBool(_saveKey, false);
            ApplyState(_element, IsSelected);
        }

        /// <summary>
        /// Applies the visual changes to the element based on its current selection state.
        /// </summary>
        /// <param name="element">The strongly-typed UI Toolkit visual element to update.</param>
        /// <param name="isSelected">The current logical selection state.</param>
        protected abstract void ApplyState(TElement element, bool isSelected);

        /// <summary>
        /// Updates the logical state, saves it to EditorPrefs, and refreshes the element's visual styling.
        /// </summary>
        /// <param name="value">The new selection state to apply.</param>
        public void SetState(bool value)
        {
            if (IsSelected == value)
                return;

            IsSelected = value;

            EditorPrefs.SetBool(_saveKey, IsSelected);
            ApplyState(_element, IsSelected);
        }

    }

}
