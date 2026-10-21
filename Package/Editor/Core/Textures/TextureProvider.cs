using System.Collections.Generic;
using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// A generic base class utilizing the Curiously Recurring Template Pattern (CRTP) 
    /// to provide a statically cached pool of textures. Limits the cache to a maximum 
    /// of two textures to prevent memory bloat in the Unity Editor.
    /// </summary>
    /// <typeparam name="TTexture">The type of Unity Texture (e.g., Texture2D, RenderTexture).</typeparam>
    /// <typeparam name="TCacher">The derived concrete implementation type.</typeparam>
    internal abstract partial class TextureProvider<TTexture, TCacher>
        where TTexture : Texture
        where TCacher  : TextureProvider<TTexture, TCacher>, new()
    {

        private static TCacher s_instance = new();

        /// <summary>
        /// A FIFO cache holding textures and their requested resolutions and HDR settings.
        /// The cache is limited to a maximum of two textures.
        /// </summary>
        private static Queue<((Vector2Int size, bool isHDR), TTexture)> s_cache;

#if !UNITY_6000_5_OR_NEWER

        /// <summary>
        /// Subscribes to assembly reload events for older Unity versions lacking the OnCodeDeinitializing attribute.
        /// </summary>
        static TextureProvider()
        {
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += ResetCache;
        }

#endif

        /// <summary>
        /// Destroys all cached textures and clears the queue.
        /// </summary>
        protected static void ResetCache()
        {
            if (s_cache == null) return;

            while (s_cache.Count > 0)
                s_instance.Destroy(s_cache.Dequeue().Item2);
        }

        /// <summary>
        /// Retrieves a texture of the requested size. If a matching texture exists in the cache, 
        /// it is returned. Otherwise, the oldest texture is destroyed (if capacity is reached) 
        /// and a new one is instantiated.
        /// </summary>
        /// <param name="screenSize">The required dimensions (width and height) of the texture.</param>
        /// <returns>An instance of <typeparamref name="TTexture"/> matching the requested size.</returns>
        public static TTexture Get(Vector2Int screenSize, bool isHDR)
        {
            s_cache ??= new Queue<((Vector2Int size, bool isHDR), TTexture)>();

            // Check if we already have a texture of the exact requested size in the cache.
            foreach (var (key, texture) in s_cache)
            {
                if (key.size == screenSize && key.isHDR == isHDR)
                    return texture;
            }

            // Limit cache size to 2 to prevent excessive VRAM/RAM usage.
            if (s_cache.Count > 1)
                s_instance.Destroy(s_cache.Dequeue().Item2);

            var newTexture = s_instance.Create(screenSize, isHDR);

            s_cache.Enqueue(((screenSize, isHDR), newTexture));

            return newTexture;
        }

        /// <summary>
        /// Factory method to create a new texture of the specified type and size.
        /// </summary>
        protected abstract TTexture Create(Vector2Int screenSize, bool isHDR);

        /// <summary>
        /// Safely destroys the texture instance to free up memory.
        /// Overridable for types that require specific disposal (e.g., RenderTexture.Release).
        /// </summary>
        protected virtual void Destroy(TTexture texture) { Object.DestroyImmediate(texture); }

    }

}
