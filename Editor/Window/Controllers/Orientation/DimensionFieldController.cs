using System;
using UnityEditor;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    internal sealed class DimensionFieldController
    {

        private readonly UnsignedIntegerField _field;
        private readonly SerializedProperty _property;

        public float Value => _property.floatValue;
        public uint UValue => (uint)MathF.Round(Value);

        public event Action<float> ValueChanged;


        public DimensionFieldController(UnsignedIntegerField field, SerializedProperty property)
        {
            _field = field;
            _property = property;

            _field.RegisterValueChangedCallback(e => OnValueChanged(e.newValue));
            _field.RegisterCallback<FocusOutEvent>(e => SetValueWithoutNotify(Value));

            SetValueWithoutNotify(Value);
        }

        private void OnValueChanged(uint value)
        {
            if (value < 1)
                return;

            SetValueWithoutNotify(value);
            ValueChanged?.Invoke(Value);
        }

        public void SetValueWithoutNotify(float value)
        {
            value = MathF.Max(value, 1f);

            _field.SetValueWithoutNotify((uint)MathF.Round(value));
            _property.floatValue = value;
            _property.serializedObject.ApplyModifiedProperties();
        }

    }

}
