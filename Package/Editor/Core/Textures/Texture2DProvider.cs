using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// A concrete provider for creating and caching Texture2D instances used for encoding images.
    /// </summary>
    internal partial class Texture2DProvider: TextureProvider<Texture2D, Texture2DProvider>
    {

#if UNITY_6000_5_OR_NEWER

        /// <summary>
        /// Safely clears the static cache during domain reload using the modern Unity API.
        /// </summary>
        [Unity.Scripting.LifecycleManagement.OnCodeDeinitializing]
        private static void Reset()
        {
            ResetCache();
        }

#endif

        /// <inheritdoc />
        protected override Texture2D Create(Vector2Int screenSize, bool isHDR)
        {
            TextureFormat format = isHDR ? TextureFormat.RGBAHalf : TextureFormat.RGB24;
            return new Texture2D(screenSize.x, screenSize.y, format, false, isHDR);
        }

    }

}
