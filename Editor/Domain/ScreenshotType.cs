using UnityEngine;


namespace Screenshots.Editor
{

    public enum ScreenshotType
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
