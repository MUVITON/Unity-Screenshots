using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace Screenshots.Editor
{

    internal static class ScreenshotFactory
    {

        private readonly struct CameraTextureScope : IDisposable
        {

            private readonly Camera _camera;
            private readonly RenderTexture _texture;

            public CameraTextureScope(Camera camera, RenderTexture texture)
            {
                _camera  = camera;
                _texture = camera.targetTexture;

                camera.targetTexture = texture;
            }

            public void Dispose() 
            {
                _camera.targetTexture = _texture;
            }

        }


        private static bool s_inProcess;

        private static Texture2D Get2DTexture(RenderTexture renderTexture)
        {
            RenderTexture.active = renderTexture;

            Texture2D texture2D = Texture2DProvider.Get(new(renderTexture.width, renderTexture.height));
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = null;

            return texture2D;
        }

        private static byte[] EncodeTexture(Texture2D texture, ScreenshotType type)
        {
            return type switch
            {
                ScreenshotType.png => texture.EncodeToPNG(),
                ScreenshotType.jpg => texture.EncodeToJPG(),
                ScreenshotType.tga => texture.EncodeToTGA(),
                ScreenshotType.exp => texture.EncodeToEXR(),
                _ => throw new NotImplementedException(nameof(type))
            };
        }

        private static void ImportScreenshot(string path)
        {
            string normalizedPath = path.Replace('\\', '/');
            if (normalizedPath.StartsWith(Application.dataPath))
            {
                string relativePath = "Assets" + normalizedPath.Substring(Application.dataPath.Length);
                AssetDatabase.ImportAsset(relativePath);
            }
            else if (normalizedPath.StartsWith("Assets/"))
            {
                AssetDatabase.ImportAsset(normalizedPath);
            }
        }

        public static async Task<string> MakeScreenshot(
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
                RenderTexture texture = RenderTextureProvider.Get(size);

                using (new CameraTextureScope(camera, texture))
                    camera.Render();

                byte[] bytes = EncodeTexture(Get2DTexture(texture), type);

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
