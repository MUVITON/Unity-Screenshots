using System;
using System.IO;
using UnityEditor;


namespace Screenshots.Editor
{

    /// <summary>
    /// Handles the safe deletion of screenshot files, distinguishing between internal Unity assets 
    /// (requiring AssetDatabase operations) and external system files.
    /// </summary>
    internal static class ScreenshotWiper
    {

        /// <summary>
        /// Deletes a file directly from the operating system's disk.
        /// </summary>
        /// <param name="path">The absolute path to the file.</param>
        /// <returns>True if the deletion was successful.</returns>
        private static bool DeleteFromDisk(string path)
        {
            File.Delete(path);
            ScreenshotsLogger.Log($"Successfully deleted external file: {path}");
            return true;
        }

        /// <summary>
        /// Deletes an asset through Unity's AssetDatabase, which safely removes both the file 
        /// and its associated .meta file.
        /// </summary>
        /// <param name="path">The relative path starting with 'Assets/'.</param>
        /// <returns>True if the asset was successfully deleted.</returns>
        private static bool DeleteFromAssetDatabase(string path)
        {
            bool result = AssetDatabase.DeleteAsset(path);

            if (result)
                ScreenshotsLogger.Log($"Successfully deleted asset and its meta file: {path}");
            else
                ScreenshotsLogger.Warn($"AssetDatabase failed to delete asset: {path}");

            return result;
        }

        /// <summary>
        /// Attempts to safely delete a screenshot at the given path. Automatically routes the request 
        /// to the AssetDatabase if the file is inside the project, or to the system IO if it's external.
        /// </summary>
        /// <param name="path">The absolute path of the screenshot to delete.</param>
        /// <returns>True if the file was deleted or already didn't exist, false if an error occurred.</returns>
        public static bool DeleteScreenshot(string path)
        {
            if (!File.Exists(path))
            {
                ScreenshotsLogger.Warn($"Cannot delete screenshot. File not found: {path}");
                // Return true because the end goal (file not being there) is already met.
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
