using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Controls the UI state and CSS classes for the screenshot orientation selector.
    /// Bridges the orientation field with the size controller to provide final dimensions.
    /// </summary>
    internal sealed class OrientationController
    {

        /// <summary>
        /// Maps each orientation mode to its corresponding CSS class name for UI styling.
        /// </summary>
        private readonly Dictionary<ScreenshotOrientation, string> _modeClassNames = new()
        {
            { ScreenshotOrientation.Landscape, "orientation--landscape" },
            { ScreenshotOrientation.Portrait,  "orientation--portrait"  },
            { ScreenshotOrientation.Custom,    "orientation--custom"  }
        };


        private readonly VisualElement _block;
        private readonly EnumField _field;
        private readonly SizeController _size;

        /// <summary>
        /// Gets the currently selected orientation mode from the UI field.
        /// </summary>
        public ScreenshotOrientation Value => (ScreenshotOrientation)_field.value;

        /// <summary>
        /// Gets the final calculated pixel dimensions based on the current orientation and resolution settings.
        /// </summary>
        public Vector2Int Size => _size.GetValue(Value);

        /// <summary>
        /// Invoked whenever the selected orientation changes or the underlying resolution size is modified.
        /// </summary>
        public event Action<Vector2Int> Updated;


        /// <summary>
        /// Initializes the controller, binds the enum field, and sets up the nested size controller.
        /// </summary>
        /// <param name="block">The main visual element containing the orientation UI.</param>
        /// <param name="field">The enum field used to select the orientation.</param>
        /// <param name="customSizeProperty">The serialized property for custom resolution dimensions.</param>
        public OrientationController(VisualElement block, EnumField field, SerializedProperty customSizeProperty)
        {
            _block = block;

            _field = field;
            _field.RegisterValueChangedCallback(e =>
            {
                AlignModeClassName(e.newValue);
                Updated?.Invoke(Size);
            });

            _size = new SizeController(block, customSizeProperty);
            _size.Updated += s => Updated?.Invoke(s);

            AlignModeClassName(_field.value);
        }

        /// <summary>
        /// Updates the CSS classes on the main block to reflect the active orientation mode.
        /// Ensures only the class corresponding to the newly selected mode is applied.
        /// </summary>
        /// <param name="value">The newly selected orientation enum value.</param>
        private void AlignModeClassName(Enum value)
        {
            ScreenshotOrientation newMode = (ScreenshotOrientation)value;

            foreach ((ScreenshotOrientation mode, string className) in _modeClassNames)
            {
                if (mode == newMode)
                    _block.AddToClassList(className);
                else
                    _block.RemoveFromClassList(className);
            }
        }

    }

}
