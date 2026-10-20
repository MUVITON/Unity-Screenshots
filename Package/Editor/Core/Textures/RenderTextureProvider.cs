using Unity.Scripting.LifecycleManagement;
using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// A concrete provider for creating and caching RenderTextures used as camera targets.
    /// </summary>
    internal partial class RenderTextureProvider: TextureProvider<RenderTexture, RenderTextureProvider>
    {

#if UNITY_6000_5_OR_NEWER

        /// <summary>
        /// Safely clears the static cache during domain reload using the modern Unity API.
        /// </summary>
        [OnCodeDeinitializing]
        private static void Reset()
        {
            ResetCache();
        }

#endif

        /// <inheritdoc />
        protected override RenderTexture Create(Vector2Int screenSize)
        {
            // 24-bit depth buffer, ARGB32 format, sRGB read/write for accurate color capture.
            return new RenderTexture(screenSize.x, screenSize.y, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        }

        /// <inheritdoc />
        protected override void Destroy(RenderTexture texture)
        {
            // RenderTexture requires explicit Release() before being destroyed to free GPU memory.
            texture.Release();
            base.Destroy(texture);
        }

    }

}
