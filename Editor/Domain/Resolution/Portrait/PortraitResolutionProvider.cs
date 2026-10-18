using System.Collections.Generic;


namespace Screenshots.Editor
{

    internal static class PortraitResolutionProvider
    {

        private static readonly Dictionary<PortraitResolution, (int, int)> _table = new()
            {
                { PortraitResolution.IPhoneProMax,    (1290, 2796) },
                { PortraitResolution.IPhonePro,       (1179, 2556) },
                { PortraitResolution.IPhoneStandard,  (1170, 2532) },
                { PortraitResolution.IPhoneSE,        (750,  1334)  },
                { PortraitResolution.AndroidFlagship, (1440, 3088) },
                { PortraitResolution.AndroidStandard, (1080, 2400) },
                { PortraitResolution.Classic1080p,    (1080, 1920) },
                { PortraitResolution.IPadPro,         (2048, 2732) }
            };

        public static (int width, int height) GetSize(PortraitResolution type)
        {
            if (_table.TryGetValue(type, out var size))
            {
                return size;
            }

            return (1080, 1920);
        }

    }

}
