using System;
using System.IO;
using UnityEditor;


namespace Screenshots.Editor
{

    internal static class ScreenshotWiper
    {

        private static bool DeleteFromDisk(string path)
        {
            File.Delete(path);
            ScreenshotsLogger.Log($"Successfully deleted external file: {path}");
            return true;
        }

        private static bool DeleteFromAssetDatabase(string path)
        {
            bool result = AssetDatabase.DeleteAsset(path);

            if (result)
                ScreenshotsLogger.Log($"Successfully deleted asset and its meta file: {path}");
            else
                ScreenshotsLogger.Warn($"AssetDatabase failed to delete asset: {path}");

            return result;
        }

        public static bool DeleteScreenshot(string path)
        {
            if (!File.Exists(path))
            {
                ScreenshotsLogger.Warn($"Cannot delete screenshot. File not found: {path}");
                return true;
            }

            try
            {
                string relativePath = ProjectPathProvider.GetForAsset(path);

                if (!string.IsNullOrEmpty(relativePath))
                    return DeleteFromAssetDatabase(relativePath);
                else
                    return DeleteFromDisk(path);
            }
            catch (Exception e)
            {
                ScreenshotsLogger.Error($"Failed to delete screenshot at '{path}': {e.Message}");
                return false;
            }
        }

    }

}
