using System.Collections.Generic;
using UnityEngine;


namespace Screenshots.Editor
{

    internal static class LandscapeResolutionProvider
    {

        private static readonly Dictionary<LandscapeResolution, Vector2Int> _table = new()
            {
                { LandscapeResolution.HD,             new(1280, 720)  },
                { LandscapeResolution.FullHD,         new(1920, 1080) },
                { LandscapeResolution.QHD,            new(2560, 1440) },
                { LandscapeResolution.UltraHD4K,      new(3840, 2160) },
                { LandscapeResolution.UltraHD8K,      new(7680, 4320) },
                { LandscapeResolution.Ultrawide1080p, new(2560, 1080) },
                { LandscapeResolution.Ultrawide1440p, new(3440, 1440) }
            };

        public static Vector2Int GetSize(LandscapeResolution type)
        {
            if (_table.TryGetValue(type, out var size))
            {
                return size;
            }

            return new(1920, 1080);
        }

    }

}
