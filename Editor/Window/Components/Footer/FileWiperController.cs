using System;
using System.Collections.Generic;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    [Serializable]
    public sealed class FileWiperController
    {

        private List<string> _paths = new();

        private Button _removeLastBtn;
        private Button _removeAllBtn;


        public void Bind(Button removeLastBtn, Button removeAllBtn)
        {
            _removeLastBtn = removeLastBtn;
            _removeAllBtn = removeAllBtn;

            _removeLastBtn.RegisterCallback<ClickEvent>(e => RemoveLast());
            _removeAllBtn.RegisterCallback<ClickEvent>(e => RemoveAll());

            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            bool isEnabled = _paths.Count != 0;

            _removeLastBtn.SetEnabled(isEnabled);
            _removeAllBtn.SetEnabled(isEnabled);
        }

        private bool RemoveLast()
        {
            int index = _paths.Count - 1;
            string path = _paths[index];
            _paths.RemoveAt(index);

            UpdateButtonsState();

            return ScreenshotWiper.DeleteScreenshot(path);
        }

        private bool RemoveAll()
        {
            while (_paths.Count > 0)
            {
                if (!RemoveLast())
                    return false ;
            }

            return true;
        }

        public void AddPath(string path)
        {
            _paths.Add(path ?? throw new ArgumentNullException(nameof(path)));
            UpdateButtonsState();
        }
     
    }

}
