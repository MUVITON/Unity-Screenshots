using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Coordinates the retrieval and updating of screenshot dimensions across different orientation modes.
    /// Acts as a central hub for landscape presets, portrait presets, and custom resolution settings.
    /// </summary>
    internal sealed class SizeController
    {

        private readonly CustomResolutionController _customResolution;

        /// <summary>
        /// Invoked whenever the selected dimension changes, regardless of the active orientation mode.
        /// Passes the newly calculated Vector2Int size as an argument.
        /// </summary>
        public event Action<Vector2Int> Updated;


        /// <summary>
        /// Initializes the controller, binds the resolution UI elements, and sets up event forwarding.
        /// </summary>
        /// <param name="block">The visual element containing the orientation-specific resolution fields.</param>
        /// <param name="customResolutionProp">The serialized property handling the custom resolution data.</param>
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

        /// <summary>
        /// Retrieves the precise pixel dimensions based on the currently requested orientation mode.
        /// </summary>
        /// <param name="orientation">The target orientation mode to evaluate.</param>
        /// <returns>A Vector2Int representing the width and height of the screenshot.</returns>
        /// <exception cref="InvalidOperationException">Thrown if an unsupported orientation value is provided.</exception>
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
