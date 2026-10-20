using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    internal sealed class SizeController
    {

        private readonly CustomResolutionController _customResolution;


        public SizeController(VisualElement block, SerializedProperty customResolutionProp) 
        {

            _customResolution = new CustomResolutionController(
                block.Q<VisualElement>("custom-controller"),
                customResolutionProp
            );

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
