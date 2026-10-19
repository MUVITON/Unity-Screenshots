using System.Collections.Generic;
using UnityEngine;


namespace Screenshots.Editor
{

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
