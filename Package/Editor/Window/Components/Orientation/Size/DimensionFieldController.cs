using System;
using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Bridges an unsigned integer UI field with a float-based serialized property (e.g., a Vector2 axis).
    /// Ensures that dimension values are safely rounded and never fall below 1 pixel.
    /// </summary>
    internal sealed class DimensionFieldController
    {

        private readonly UnsignedIntegerField _field;
        private readonly SerializedProperty _property;

        /// <summary>
        /// Gets the underlying float value directly from the serialized property.
        /// </summary>
        public float Value => _property.floatValue;

        /// <summary>
        /// Gets the rounded unsigned integer representation of the current property value.
        /// </summary>
        public uint UValue => (uint)MathF.Round(Value);

        /// <summary>
        /// Invoked whenever the dimension value is successfully changed and validated through the UI.
        /// </summary>
        public event Action<float> ValueChanged;


        /// <summary>
        /// Initializes the controller, binds the UI field to the serialized property, and sets up validation events.
        /// </summary>
        /// <param name="field">The UI Toolkit integer field for user input.</param>
        /// <param name="property">The serialized float property representing a single axis dimension.</param>
        public DimensionFieldController(UnsignedIntegerField field, SerializedProperty property)
        {
            _field = field;
            _property = property;

            _field.RegisterValueChangedCallback(e => OnValueChanged(e.newValue));
            _field.RegisterCallback<FocusOutEvent>(e => SetValueWithoutNotify(Value));

            SetValueWithoutNotify(Value);
        }

        /// <summary>
        /// Validates user input to prevent dimensions smaller than 1 pixel.
        /// Syncs the valid value and triggers the change event.
        /// </summary>
        /// <param name="value">The new unsigned integer value inputted by the user.</param>
        private void OnValueChanged(uint value)
        {
            if (value < 1)
                return;

            SetValueWithoutNotify(value);
            ValueChanged?.Invoke(Value);
        }

        /// <summary>
        /// Updates both the UI field and the serialized property without triggering recursive change events.
        /// Clamps the minimum value to 1 to prevent rendering errors with zero-sized textures.
        /// </summary>
        /// <param name="value">The raw float dimension to apply.</param>
        public void SetValueWithoutNotify(float value)
        {
            value = MathF.Max(value, 1f);

            _field.SetValueWithoutNotify((uint)MathF.Round(value));
            _property.floatValue = value;
            _property.serializedObject.ApplyModifiedProperties();
        }

    }

}
