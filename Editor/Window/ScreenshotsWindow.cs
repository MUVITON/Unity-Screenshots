using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Editor window for capturing screenshots.
    /// Provides options to select a camera source, screen orientation, and destination folder.
    /// </summary>
    public class ScreenshotsWindow : EditorWindow
    {

        #region State

        [SerializeField, Tooltip("Visual Tree Asset for the Screenshots Window UI")]
        private VisualTreeAsset _screenshotsWindowUXMLAsset;

        private CameraController _camera   = new();
        private FileWiperController _wiper = new();
        private OrientationController _orientation;
        private ActiveFolderPathController _folderPath;
        private FileNameController _fileName;
        private Button _screenshotBtn;

        #endregion


        #region Initialization

        /// <summary>
        /// Opens the Screen Record window.
        /// </summary>
        [MenuItem("Tools/Screenshots")]
        public static void OpenWindow()
        {
            var window = GetWindow<ScreenshotsWindow>("Screenshots");
            window.minSize = new Vector2(580, 300);
            window.Show();
        }

        /// <summary>
        /// Creates and initializes the UI elements of the window.
        /// </summary>
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            var so = new SerializedObject(ScreenshotsConfig.instance);

            root.Add(_screenshotsWindowUXMLAsset.Instantiate());
            root.Bind(so);
            root.RegisterCallback<KeyDownEvent>(KeyDownHandler);

            _camera.Bind(root.Q<VisualElement>("camera-block"));

            _orientation = new OrientationController(
                root.Q<VisualElement>("orientation-block"),
                root.Q<EnumField>("orientation-selector"),
                so.FindProperty("<CustomResolution>k__BackingField")
            );

            new DefaultFolderPathController(
                root.Q<FolderField>("default-folder-path"), 
                so.FindProperty("<DefaultFolderPath>k__BackingField")
            );

            _folderPath = new ActiveFolderPathController(
                root.Q<FolderField>("active-folder-path"),
                so.FindProperty("<FolderPath>k__BackingField"),
                so.FindProperty("<DefaultFolderPath>k__BackingField")
            );

            _fileName = new FileNameController(
                root.Q<VisualElement>("file-name__controller"));

            _wiper.Bind(
                root.Q<Button>("remove-last-btn"),
                root.Q<Button>("remove-all-btn")
            );

            _screenshotBtn = root.Q<Button>("screenshot-btn");
            _screenshotBtn.clicked += CaptureScreenshot;
        }

        /// <summary>
        /// Handles key down events. Triggers screenshot capture when Ctrl+E is pressed.
        /// </summary>
        /// <param name="e">The key down event data.</param>
        private void KeyDownHandler(KeyDownEvent e)
        {
            if (e.ctrlKey && e.keyCode == KeyCode.E)
                CaptureScreenshot();
        }

        #endregion


        #region Disable

        private void OnDisable()
        {
            _camera.Dispose();
            _folderPath.Dispose();
        }

        #endregion


        #region Capture Handler

        /// <summary>
        /// Captures a screenshot using the selected camera and saves it to the specified folder.
        /// </summary>
        private async void CaptureScreenshot()
        {
            _screenshotBtn.SetEnabled(false);

            try
            {
                string path = await ScreenshotFactory.CaptureScreenshot(
                    _camera.Reference,
                    _orientation.Size,
                    _folderPath.Path,
                    _fileName.Value,
                    _fileName.Type
                );

                if (!string.IsNullOrEmpty(path))
                    _wiper.AddPath(path);
            }
            finally
            {
                _screenshotBtn.SetEnabled(true);
            }
        }

        #endregion

    }

}
