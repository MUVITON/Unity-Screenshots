using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace Screenshots.Editor
{

    /// <summary>
    /// Core engine for capturing, encoding, and saving camera views as image files.
    /// </summary>
    internal static class ScreenshotFactory
    {

        /// <summary>
        /// A helper struct that safely swaps a camera's target texture and restores the original one upon disposal.
        /// </summary>
        private readonly struct CameraTextureScope : IDisposable
        {

            private readonly Camera _camera;
            private readonly RenderTexture _texture;

            public CameraTextureScope(Camera camera, RenderTexture texture)
            {
                _camera  = camera;
                _texture = _camera.targetTexture;

                _camera.targetTexture = texture;
            }

            public void Dispose() 
            {
                _camera.targetTexture = _texture;
            }

        }

        /// <summary>
        /// A helper struct that safely sets the active <see cref="RenderTexture"/> 
        /// and restores the previously active one upon disposal.
        /// </summary>
        private readonly struct RenderTextureScope: IDisposable
        {

            private readonly RenderTexture _previous;

            public RenderTextureScope(RenderTexture texture)
            {
                _previous = RenderTexture.active;
                RenderTexture.active = texture;
            }

            public void Dispose()
            {
                RenderTexture.active = _previous;
            }

        }


        // Flag to prevent multiple simultaneous screenshot capture operations.
        private static bool s_inProcess;

        /// <summary>
        /// Reads pixels from the provided RenderTexture into a Texture2D.
        /// </summary>
        private static Texture2D Get2DTexture(RenderTexture renderTexture, bool isHDR)
        {
            using var scope = new RenderTextureScope(renderTexture);

            Texture2D texture2D = Texture2DProvider.Get(
                    new(renderTexture.width, renderTexture.height), isHDR);
            texture2D.ReadPixels(
                new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            return texture2D;
        }

        /// <summary>
        /// Encodes a Texture2D into a byte array based on the specified image format.
        /// </summary>
        private static byte[] EncodeTexture(Texture2D texture, ScreenshotType type)
        {
            return type switch
            {
                ScreenshotType.png => texture.EncodeToPNG(),
                ScreenshotType.jpg => texture.EncodeToJPG(),
                ScreenshotType.tga => texture.EncodeToTGA(),
                ScreenshotType.exr => texture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported screenshot format.")
            };
        }

        /// <summary>
        /// Prompts the AssetDatabase to import the newly created screenshot if it was saved within the project's Assets folder.
        /// </summary>
        private static void ImportScreenshot(string path)
        {
            string relativePath = ProjectPathProvider.GetForAsset(path);

            if (!string.IsNullOrEmpty(relativePath))
                AssetDatabase.ImportAsset(relativePath);
        }

        /// <summary>
        /// Asynchronously captures a screenshot from the specified camera and saves it to disk.
        /// </summary>
        /// <param name="camera">The camera to capture the screenshot from.</param>
        /// <param name="size">The resolution of the screenshot in pixels.</param>
        /// <param name="folderPath">The absolute or relative directory path to save the file.</param>
        /// <param name="fileName">The formatted file name (without extension).</param>
        /// <param name="type">The image format (e.g., png, jpg).</param>
        /// <returns>The full path of the saved screenshot, or null if the capture failed or was cancelled.</returns>
        public static async Task<string> CaptureScreenshot(
            Camera camera,
            Vector2Int size,
            string folderPath,
            string fileName,
            ScreenshotType type
        )
        {
            if (s_inProcess)
            {
                ScreenshotsLogger.Warn("A screenshot capture is already in progress. Please wait.");
                return null;
            }

            string path = Path.Combine(folderPath, $"{fileName}.{type}");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (File.Exists(path))
            {
                ScreenshotsLogger.Warn($"File '{fileName}.{type}' already exists. Capture cancelled to prevent overwriting.");
                return null;
            }

            s_inProcess = true;

            try
            {
                bool isHDR = type == ScreenshotType.exr;
                RenderTexture texture = RenderTextureProvider.Get(size, isHDR);

                using (new CameraTextureScope(camera, texture))
                    camera.Render();

                byte[] bytes = EncodeTexture(Get2DTexture(texture, isHDR), type);

                await File.WriteAllBytesAsync(path, bytes);

                ImportScreenshot(path);

                ScreenshotsLogger.Log($"Successfully saved to: {path}");
                return path;
            }
            finally
            {
                s_inProcess = false;
            }
        }

    }

}
