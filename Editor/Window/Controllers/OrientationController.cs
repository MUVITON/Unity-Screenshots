using System;
using System.Collections.Generic;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class OrientationController
    {

        private readonly Dictionary<ScreenshotOrientation, string> _modeClassNames = new()
        {
            { ScreenshotOrientation.Landscape, "orientation--landscape" },
            { ScreenshotOrientation.Portrait,  "orientation--portrait"  }
        };


        private readonly VisualElement _block;
        private readonly EnumField _field;

        public ScreenshotOrientation Value => (ScreenshotOrientation)_field.value;


        public OrientationController(VisualElement block, EnumField field)
        {
            _block = block;

            _field = field;
            _field.RegisterValueChangedCallback(e => AlignModeClassName(e.newValue));

            AlignModeClassName(_field.value);
        }

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
