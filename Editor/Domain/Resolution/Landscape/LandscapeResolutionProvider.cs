using System.Collections.Generic;


namespace Screenshots.Editor
{

    internal static class LandscapeResolutionProvider
    {

        private static readonly Dictionary<LandscapeResolution, (int, int)> _table = new()
            {
                { LandscapeResolution.HD,             (1280, 720)  },
                { LandscapeResolution.FullHD,         (1920, 1080) },
                { LandscapeResolution.QHD,            (2560, 1440) },
                { LandscapeResolution.UltraHD4K,      (3840, 2160) },
                { LandscapeResolution.UltraHD8K,      (7680, 4320) },
                { LandscapeResolution.Ultrawide1080p, (2560, 1080) },
                { LandscapeResolution.Ultrawide1440p, (3440, 1440) }
            };

        public static (int width, int height) GetSize(LandscapeResolution type)
        {
            if (_table.TryGetValue(type, out var size))
            {
                return size;
            }

            return (1920, 1080);
        }

    }

}
