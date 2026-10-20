using System;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// The main Editor window for capturing screenshots.
    /// Provides a unified UI to select a camera source, screen orientation, and destination folder.
    /// </summary>
    public sealed class ScreenshotsWindow : EditorWindow
    {

        #region State

        private static Vector2 MinWindowSize     = new Vector2(580, 350);
        private static Vector2 DesiredWindowSize = new Vector2(580, 520);


        [SerializeField, Tooltip("Visual Tree Asset for the Screenshots Window UI")]
        private VisualTreeAsset _screenshotsWindowUXMLAsset;

        private CameraController _camera   = new();
        private FileWiperController _wiper = new();
        private OrientationController _orientation;
        private FileFolderController _fileFolder;
        private FileNameController _fileName;
        private Button _screenshotBtn;

        /// <summary>
        /// Tracks whether a screenshot capture is currently in progress to prevent overlapping requests.
        /// </summary>
        [NonSerialized]
        private bool _inProcess;

        #endregion


        #region Initialization

        /// <summary>
        /// Opens the Screenshots tool window from the Unity top menu.
        /// </summary>
        [MenuItem("Tools/Screenshots")]
        public static void OpenWindow()
        {
            var window = GetWindow<ScreenshotsWindow>("Screenshots");

            window.minSize = MinWindowSize;

            Rect mainWindow = EditorGUIUtility.GetMainWindowPosition();

            float x = mainWindow.x + (mainWindow.width - DesiredWindowSize.x) * 0.5f;
            float y = mainWindow.y + (mainWindow.height - DesiredWindowSize.y) * 0.5f;

            window.position = new Rect(x, y, DesiredWindowSize.x, DesiredWindowSize.y);

            window.Show();
        }

        /// <summary>
        /// Creates and initializes the UI elements of the window, binding them to the configuration singleton.
        /// </summary>
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            var so = new SerializedObject(ScreenshotsConfig.instance);

            root.Add(_screenshotsWindowUXMLAsset.Instantiate());
            root.Bind(so);

            _camera.Bind(root.Q<VisualElement>("camera-block"));

            _orientation = new OrientationController(
                root.Q<VisualElement>("orientation-block"),
                root.Q<EnumField>("orientation-selector"),
                so.FindProperty("<CustomResolution>k__BackingField")
            );
            _orientation.Updated += s => _fileName.UpdateSize(s);

            _fileFolder = new FileFolderController(
                root.Q<VisualElement>("folder-block"),
                so.FindProperty("<FolderPath>k__BackingField"),
                so.FindProperty("<DefaultFolderPath>k__BackingField")
            );

            _fileName = new FileNameController(
                root.Q<VisualElement>("file-name__controller"),
                _orientation.Size
            );

            _wiper.Bind(
                root.Q<Button>("remove-last-btn"),
                root.Q<Button>("remove-all-btn")
            );

            _screenshotBtn = root.Q<Button>("screenshot-btn");
            _screenshotBtn.clicked += CaptureScreenshot;
        }

        /// <summary>
        /// A global shortcut (F12) to capture a screenshot without clicking the UI button.
        /// Only functions if the Screenshots window is currently active in the layout.
        /// </summary>
        [Shortcut("Screenshots/Capture Screenshot", KeyCode.F12)]
        public static void GlobalCaptureShortcut()
        {
            if (HasOpenInstances<ScreenshotsWindow>())
            {
                var window = GetWindow<ScreenshotsWindow>("Screenshots");
                window.CaptureScreenshot();
            }
        }

        #endregion


        #region Capture

        /// <summary>
        /// Captures a screenshot using the selected camera and saves it to the specified folder.
        /// </summary>
        private async void CaptureScreenshot()
        {
            if (_inProcess)
                return;

            _inProcess = true;
            _screenshotBtn.SetEnabled(false);

            try
            {
                string path = await ScreenshotFactory.CaptureScreenshot(
                    _camera.Reference,
                    _orientation.Size,
                    _fileFolder.Path,
                    _fileName.Value,
                    _fileName.Type
                );

                if (!string.IsNullOrEmpty(path))
                    _wiper.AddPath(path);
            }
            finally
            {
                _inProcess = false;
                _screenshotBtn.SetEnabled(true);
            }
        }

        #endregion


        #region Disable

        /// <summary>
        /// Cleans up resources, unsubscribes from events, and ensures memory is freed when the window closes.
        /// </summary>
        private void OnDisable()
        {
            _camera.Dispose();
            _fileFolder.Dispose();
        }

        #endregion

    }

}
