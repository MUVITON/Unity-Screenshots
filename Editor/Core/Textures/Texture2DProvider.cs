using Unity.Scripting.LifecycleManagement;
using UnityEngine;


namespace Screenshots.Editor
{

    public partial class Texture2DProvider: TextureProvider<Texture2D, Texture2DProvider>
    {

        [OnCodeDeinitializing]
        private static void Reset()
        {
            ResetCache();
        }

        protected override Texture2D Create(Vector2Int screenSize)
        {
            return new Texture2D(screenSize.x, screenSize.y, TextureFormat.RGB24, false);
        }

    }

}
