using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// Defines standard portrait (vertical) resolutions commonly used for mobile devices and tablets.
    /// </summary>
    internal enum PortraitResolution
    {

        [InspectorName("iPhone 15 & 14 Pro Max (1290 x 2796)")]
        IPhoneProMax,

        [InspectorName("iPhone 15 & 14 Pro (1179 x 2556)")]
        IPhonePro,

        [InspectorName("iPhone 13 & 14 (1170 x 2532)")]
        IPhoneStandard,

        [InspectorName("iPhone SE & 8 (750 x 1334)")]
        IPhoneSE,

        [InspectorName("Android Flagship (1440 x 3088)")]
        AndroidFlagship,

        [InspectorName("Android Standard (1080 x 2400)")]
        AndroidStandard,

        [InspectorName("Classic Portrait (1080 x 1920)")]
        Classic1080p,

        [InspectorName("iPad Pro (2048 x 2732)")]
        IPadPro

    }

}
