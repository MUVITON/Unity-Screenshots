using System.Collections.Generic;
using UnityEngine;


namespace Screenshots.Editor
{

    public abstract partial class TextureProvider<TTexture, TCacher>
        where TTexture : Texture
        where TCacher  : TextureProvider<TTexture, TCacher>, new()
    {

        private static TCacher _instance = new();

        private static Queue<(Vector2Int, TTexture)> _cache;

        protected static void ResetCache()
        {
            if (_cache == null) return;

            while (_cache.Count > 0)
                _instance.Destroy(_cache.Dequeue().Item2);
        }

        public static TTexture Get(Vector2Int screenSize)
        {
            _cache ??= new Queue<(Vector2Int, TTexture)>();

            foreach ((Vector2Int size, TTexture texture) in _cache)
            {
                if (size == screenSize)
                    return texture;
            }

            if (_cache.Count > 1)
                _instance.Destroy(_cache.Dequeue().Item2);

            var newTexture = _instance.Create(screenSize);

            _cache.Enqueue((screenSize, newTexture));

            return newTexture;
        }

        protected abstract TTexture Create(Vector2Int screenSize);

        protected virtual void Destroy(TTexture texture) { Object.DestroyImmediate(texture); }

    }

}
