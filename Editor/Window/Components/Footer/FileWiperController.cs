using System;
using System.Collections.Generic;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Manages the history of captured screenshots during the current session.
    /// Controls the UI logic for wiping (deleting) the last captured file or all tracked files.
    /// </summary>
    [Serializable]
    internal sealed class FileWiperController
    {

        private List<string> _paths = new();

        private Button _removeLastBtn;
        private Button _removeAllBtn;


        /// <summary>
        /// Binds the controller to the deletion UI buttons and initializes their interactive state.
        /// </summary>
        /// <param name="removeLastBtn">The button responsible for deleting the most recent screenshot.</param>
        /// <param name="removeAllBtn">The button responsible for clearing the entire session history.</param>
        public void Bind(Button removeLastBtn, Button removeAllBtn)
        {
            _removeLastBtn = removeLastBtn;
            _removeAllBtn = removeAllBtn;

            _removeLastBtn.RegisterCallback<ClickEvent>(e => RemoveLast());
            _removeAllBtn.RegisterCallback<ClickEvent>(e => RemoveAll());

            UpdateButtonsState();
        }

        /// <summary>
        /// Updates the enabled state of the buttons based on whether there are any tracked paths.
        /// </summary>
        private void UpdateButtonsState()
        {
            bool isEnabled = _paths.Count != 0;

            _removeLastBtn.SetEnabled(isEnabled);
            _removeAllBtn.SetEnabled(isEnabled);
        }

        /// <summary>
        /// Pops the most recent file path from the history and attempts to delete it from the disk or AssetDatabase.
        /// </summary>
        /// <returns>True if the file was successfully deleted or didn't exist, false if an error occurred.</returns>
        private bool RemoveLast()
        {
            int index = _paths.Count - 1;
            string path = _paths[index];
            _paths.RemoveAt(index);

            UpdateButtonsState();

            return ScreenshotWiper.DeleteScreenshot(path);
        }

        /// <summary>
        /// Iteratively removes all tracked screenshots from newest to oldest.
        /// Stops execution early if any deletion process fails.
        /// </summary>
        /// <returns>True if the entire history was successfully wiped, false if any deletion failed.</returns>
        private bool RemoveAll()
        {
            while (_paths.Count > 0)
            {
                if (!RemoveLast())
                    return false ;
            }

            return true;
        }

        /// <summary>
        /// Registers a newly captured screenshot path into the session history and enables the deletion buttons.
        /// </summary>
        /// <param name="path">The absolute path of the saved screenshot.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided path is null.</exception>
        public void AddPath(string path)
        {
            _paths.Add(path ?? throw new ArgumentNullException(nameof(path)));
            UpdateButtonsState();
        }
     
    }

}
