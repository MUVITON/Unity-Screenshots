using Unity.Scripting.LifecycleManagement;
using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// A concrete provider for creating and caching RenderTextures used as camera targets.
    /// </summary>
    internal partial class RenderTextureProvider: TextureProvider<RenderTexture, RenderTextureProvider>
    {

        [OnCodeDeinitializing]
        private static void Reset()
        {
            ResetCache();
        }

        protected override RenderTexture Create(Vector2Int screenSize)
        {
            // 24-bit depth buffer, ARGB32 format, sRGB read/write for accurate color capture.
            return new RenderTexture(screenSize.x, screenSize.y, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        }

        protected override void Destroy(RenderTexture texture)
        {
            // RenderTexture requires explicit Release() before being destroyed to free GPU memory.
            texture.Release();
            base.Destroy(texture);
        }

    }

}
