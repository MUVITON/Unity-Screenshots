using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class CustomResolutionController
    {

        private readonly Button _swapBtn;
        private readonly Button _resetBtn;
        private readonly IconButtonSelectionController _lockBtnCtrl;

        private readonly DimensionFieldController _xCtrl;
        private readonly DimensionFieldController _yCtrl;

        private float _aspect;

        public Vector2Int Value => new((int)_xCtrl.UValue, (int)_yCtrl.UValue);

        public event Action<Vector2Int> Updated;


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

        private void OnValueChanged()
        {
            _swapBtn.SetEnabled(_xCtrl.UValue != _yCtrl.UValue);
        }

        private void ResetAspect()
        {
            _aspect = _xCtrl.Value / _yCtrl.Value;
        }

        private void SetValueWithoutNotify(float x, float y)
        {
            _xCtrl.SetValueWithoutNotify(x);
            _yCtrl.SetValueWithoutNotify(y);

            ResetAspect();
            OnValueChanged();

            Updated?.Invoke(Value);
        }

        private void SwapDimensions()
        {
            SetValueWithoutNotify(_yCtrl.Value, _xCtrl.Value);
        }

        private void Reset()
        {
            Vector2Int size = LandscapeResolutionProvider.GetSize(LandscapeResolution.FullHD);
            SetValueWithoutNotify(size.x, size.y);
        }

        private void SetLock(bool value)
        {
            _lockBtnCtrl.SetState(value);

            if (_lockBtnCtrl.IsSelected)
                ResetAspect();
        }

    }

}
