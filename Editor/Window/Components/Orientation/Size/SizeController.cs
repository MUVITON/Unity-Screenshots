using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    internal sealed class SizeController
    {

        private readonly CustomResolutionController _customResolution;
        public event Action<Vector2Int> Updated;


        public SizeController(VisualElement block, SerializedProperty customResolutionProp) 
        {
            block.Q<EnumField>("landscape-selector")
                .RegisterValueChangedCallback(e => Updated?.Invoke(GetValue(ScreenshotOrientation.Landscape)));

            block.Q<EnumField>("portrait-selector")
                .RegisterValueChangedCallback(e => Updated?.Invoke(GetValue(ScreenshotOrientation.Portrait)));

            _customResolution = new CustomResolutionController(
                block.Q<VisualElement>("custom-controller"),
                customResolutionProp
            );

            _customResolution.Updated += s => Updated?.Invoke(s);
        }

        public Vector2Int GetValue(ScreenshotOrientation orientation)
        {
            return orientation switch
            {
                ScreenshotOrientation.Landscape => LandscapeResolutionProvider.GetSize(ScreenshotsConfig.instance.LandscapeResolution),
                ScreenshotOrientation.Portrait  => PortraitResolutionProvider.GetSize(ScreenshotsConfig.instance.PortraitResolution),
                ScreenshotOrientation.Custom    => _customResolution.Value,
                _ => throw new InvalidOperationException()
            };
        }

    }

}
