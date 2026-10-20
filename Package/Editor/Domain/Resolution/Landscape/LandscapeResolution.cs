using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// Defines standard landscape (horizontal) resolutions commonly used for desktop and web platforms.
    /// </summary>
    internal enum LandscapeResolution
    {

        [InspectorName("HD 720p (1280 x 720)")]
        HD,

        [InspectorName("Full HD 1080p (1920 x 1080)")]
        FullHD,

        [InspectorName("QHD 1440p (2560 x 1440)")]
        QHD,

        [InspectorName("4K UHD (3840 x 2160)")]
        UltraHD4K,

        [InspectorName("8K UHD (7680 x 4320)")]
        UltraHD8K,

        [InspectorName("Ultrawide 1080p (2560 x 1080)")]
        Ultrawide1080p,

        [InspectorName("Ultrawide 1440p (3440 x 1440)")]
        Ultrawide1440p

    }

}
