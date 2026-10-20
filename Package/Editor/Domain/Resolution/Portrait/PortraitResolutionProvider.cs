using System.Collections.Generic;
using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// Provides a mapping from the <see cref="PortraitResolution"/> enum values to their precise pixel dimensions.
    /// </summary>
    internal static class PortraitResolutionProvider
    {

        private static readonly Dictionary<PortraitResolution, Vector2Int> _table = new()
            {
                { PortraitResolution.IPhoneProMax,    new(1290, 2796) },
                { PortraitResolution.IPhonePro,       new(1179, 2556) },
                { PortraitResolution.IPhoneStandard,  new(1170, 2532) },
                { PortraitResolution.IPhoneSE,        new(750, 1334)  },
                { PortraitResolution.AndroidFlagship, new(1440, 3088) },
                { PortraitResolution.AndroidStandard, new(1080, 2400) },
                { PortraitResolution.Classic1080p,    new(1080, 1920) },
                { PortraitResolution.IPadPro,         new(2048, 2732) }
            };

        /// <summary>
        /// Retrieves the pixel dimensions for a specified portrait resolution.
        /// </summary>
        /// <param name="type">The selected portrait resolution preset.</param>
        /// <returns>A Vector2Int representing the width and height. Defaults to 1080x1920 if not found.</returns>
        public static Vector2Int GetSize(PortraitResolution type)
        {
            if (_table.TryGetValue(type, out var size))
            {
                return size;
            }

            return new(1080, 1920);
        }

    }

}
