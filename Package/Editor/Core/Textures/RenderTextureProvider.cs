using System;
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
        [Unity.Scripting.LifecycleManagement.OnCodeDeinitializing]
        private static void Reset()
        {
            ResetCache();
        }

#endif

        /// <inheritdoc />
        protected override RenderTexture Create(Vector2Int screenSize, bool isHDR)
        {
            var format    = isHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.ARGB32;
            var readWrite = isHDR ? RenderTextureReadWrite.Linear  : RenderTextureReadWrite.sRGB;

            return new RenderTexture(screenSize.x, screenSize.y, 24, format, readWrite);
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
