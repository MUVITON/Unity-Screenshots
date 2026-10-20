using Unity.Scripting.LifecycleManagement;
using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// A concrete provider for creating and caching Texture2D instances used for encoding images.
    /// </summary>
    internal partial class Texture2DProvider: TextureProvider<Texture2D, Texture2DProvider>
    {

        [OnCodeDeinitializing]
        private static void Reset()
        {
            ResetCache();
        }

        protected override Texture2D Create(Vector2Int screenSize)
        {
            // RGB24 is sufficient for standard screenshots without an alpha channel, saving memory.
            return new Texture2D(screenSize.x, screenSize.y, TextureFormat.RGB24, false);
        }

    }

}
