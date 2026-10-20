using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// Defines the supported image file formats for saving screenshots.
    /// </summary>
    internal enum ScreenshotType
    {

        [InspectorName("png")]
        png,

        [InspectorName("jpg")]
        jpg,

        [InspectorName("tga")]
        tga,

        [InspectorName("exr")]
        exr

    }

}
