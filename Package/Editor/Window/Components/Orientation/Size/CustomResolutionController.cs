using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Controls the UI logic for defining custom screenshot resolutions, including aspect ratio locking, dimension swapping, and resetting.
    /// </summary>
    internal sealed class CustomResolutionController
    {

        private readonly Button _swapBtn;
        private readonly Button _resetBtn;
        private readonly IconButtonSelectionController _lockBtnCtrl;

        private readonly DimensionFieldController _xCtrl;
        private readonly DimensionFieldController _yCtrl;

        private float _aspect;

        /// <summary>
        /// Gets the current custom resolution dimensions as a Vector2Int.
        /// </summary>
        public Vector2Int Value => new((int)_xCtrl.UValue, (int)_yCtrl.UValue);

        /// <summary>
        /// Invoked whenever the custom resolution dimensions are modified.
        /// </summary>
        public event Action<Vector2Int> Updated;


        /// <summary>
        /// Initializes the controller, binds the UI elements, and sets up dimension tracking and aspect ratio locking logic.
        /// </summary>
        /// <param name="container">The root visual element containing the resolution controls.</param>
        /// <param name="property">The serialized Vector2Int property storing the custom resolution.</param>
        public CustomResolutionController(VisualElement container, SerializedProperty property)
        {
            var widthField  = container.Q<UnsignedIntegerField>("width-field");
            var heightField = container.Q<UnsignedIntegerField>("height-field");

            SerializedProperty xProp = property.FindPropertyRelative("x");
            SerializedProperty yProp = property.FindPropertyRelative("y");

            var lockBtn = container.Q<Button>("lock-btn");
            _lockBtnCtrl = new IconButtonSelectionController(lockBtn);
            lockBtn.RegisterCallback<ClickEvent>(e => SetLock(!_lockBtnCtrl.IsSelected));

            _swapBtn  = container.Q<Button>("swap-btn");
            _resetBtn = container.Q<Button>("reset-btn");

            _swapBtn.RegisterCallback<ClickEvent>(e => SwapDimensions());
            _resetBtn.RegisterCallback<ClickEvent>(e => Reset());

            _xCtrl = new DimensionFieldController(widthField,  xProp);
            _yCtrl = new DimensionFieldController(heightField, yProp);

            _xCtrl.ValueChanged += x =>
            {
                if (_lockBtnCtrl.IsSelected && _aspect > 0f)
                    _yCtrl.SetValueWithoutNotify(x / _aspect);

                OnValueChanged();
                Updated?.Invoke(Value);
            };

            _yCtrl.ValueChanged += y =>
            {
                if (_lockBtnCtrl.IsSelected && _aspect > 0f)
                    _xCtrl.SetValueWithoutNotify(y * _aspect);

                OnValueChanged();
                Updated?.Invoke(Value);
            };

            ResetAspect();
            OnValueChanged();
        }

        /// <summary>
        /// Evaluates the current dimensions to enable or disable the swap button.
        /// </summary>
        private void OnValueChanged()
        {
            _swapBtn.SetEnabled(_xCtrl.UValue != _yCtrl.UValue);
        }

        /// <summary>
        /// Recalculates and caches the aspect ratio based on the current width and height.
        /// </summary>
        private void ResetAspect()
        {
            _aspect = _xCtrl.Value / _yCtrl.Value;
        }

        /// <summary>
        /// Programmatically updates both dimensions without triggering their internal change callbacks, then fires the main update event.
        /// </summary>
        /// <param name="x">The new width value.</param>
        /// <param name="y">The new height value.</param>
        private void SetValueWithoutNotify(float x, float y)
        {
            _xCtrl.SetValueWithoutNotify(x);
            _yCtrl.SetValueWithoutNotify(y);

            ResetAspect();
            OnValueChanged();

            Updated?.Invoke(Value);
        }

        /// <summary>
        /// Inverts the width and height values, adjusting the aspect ratio accordingly.
        /// </summary>
        private void SwapDimensions()
        {
            SetValueWithoutNotify(_yCtrl.Value, _xCtrl.Value);
        }

        /// <summary>
        /// Reverts the custom dimensions to the standard Full HD (1920x1080) preset.
        /// </summary>
        private void Reset()
        {
            Vector2Int size = LandscapeResolutionProvider.GetSize(LandscapeResolution.FullHD);
            SetValueWithoutNotify(size.x, size.y);
        }

        /// <summary>
        /// Toggles the aspect ratio lock state and recalculates the base ratio if locked.
        /// </summary>
        /// <param name="value">True to lock the aspect ratio, false to unlock.</param>
        private void SetLock(bool value)
        {
            _lockBtnCtrl.SetState(value);

            if (_lockBtnCtrl.IsSelected)
                ResetAspect();
        }

    }

}
