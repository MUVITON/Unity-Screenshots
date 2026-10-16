using Unity.Scripting.LifecycleManagement;
using UnityEngine;


namespace Screenshots.Editor
{

    public partial class RenderTextureProvider: TextureProvider<RenderTexture, RenderTextureProvider>
    {

        [OnCodeDeinitializing]
        private static void Reset()
        {
            ResetCache();
        }

        protected override RenderTexture Create(Vector2Int screenSize)
        {
            return new RenderTexture(screenSize.x, screenSize.y, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        }

        protected override void Destroy(RenderTexture texture)
        {
            texture.Release();
            base.Destroy(texture);
        }

    }

}
