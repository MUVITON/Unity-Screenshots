using System.Collections.Generic;
using UnityEngine;


namespace Screenshots.Editor
{

    public abstract partial class TextureProvider<TTexture, TCacher>
        where TTexture : Texture
        where TCacher  : TextureProvider<TTexture, TCacher>, new()
    {

        private static TCacher s_instance = new();

        private static Queue<(Vector2Int, TTexture)> s_cache;

        protected static void ResetCache()
        {
            if (s_cache == null) return;

            while (s_cache.Count > 0)
                s_instance.Destroy(s_cache.Dequeue().Item2);
        }

        public static TTexture Get(Vector2Int screenSize)
        {
            s_cache ??= new Queue<(Vector2Int, TTexture)>();

            foreach ((Vector2Int size, TTexture texture) in s_cache)
            {
                if (size == screenSize)
                    return texture;
            }

            if (s_cache.Count > 1)
                s_instance.Destroy(s_cache.Dequeue().Item2);

            var newTexture = s_instance.Create(screenSize);

            s_cache.Enqueue((screenSize, newTexture));

            return newTexture;
        }

        protected abstract TTexture Create(Vector2Int screenSize);

        protected virtual void Destroy(TTexture texture) { Object.DestroyImmediate(texture); }

    }

}
