using System.IO;
using System.Text.RegularExpressions;
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

        [SerializeField, Tooltip("Visual Tree Asset for the Screen Record Window UI")]
        private VisualTreeAsset _screenRecordWindowUXMLAsset;

        private TextField _folderPathField;

        private Camera _camera;

        #endregion


        #region Initialization

        /// <summary>
        /// Opens the Screen Record window.
        /// </summary>
        [MenuItem("Tools/Screenshots")]
        public static void OpenWindow()
        {
            GetWindow<ScreenshotsWindow>("Screenshots").Show();
        }

        /// <summary>
        /// Creates and initializes the UI elements of the window.
        /// </summary>
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.Add(_screenRecordWindowUXMLAsset.Instantiate());
            root.RegisterCallback<KeyDownEvent>(KeyDownHandler);

            InitCameraField(root);
            InitScreenshotOrientation(root);
            InitFolderPathSelector(root);
            InitFooter(root);
        }

        /// <summary>
        /// Initializes the camera selection field.
        /// </summary>
        /// <param name="root">The root VisualElement of the window.</param>
        private void InitCameraField(VisualElement root)
        {
            ObjectField cameraField = root.Q<ObjectField>("camera-source");
            cameraField.RegisterValueChangedCallback(e => { _camera = e.newValue as Camera; });
        }

        /// <summary>
        /// Initializes the screen orientation field and render texture.
        /// </summary>
        /// <param name="root">The root VisualElement of the window.</param>
        private void InitScreenshotOrientation(VisualElement root)
        {
            var field = root.Q<EnumField>("orientation-field");
            field.BindProperty(new SerializedObject(ScreenshotsConfig.Instance));
        }

        /// <summary>
        /// Initializes the folder path selector.
        /// </summary>
        /// <param name="root">The root VisualElement of the window.</param>
        private void InitFolderPathSelector(VisualElement root)
        {
            _folderPathField = root.Q<TextField>("folder-path-field");
            _folderPathField.SetValueWithoutNotify(!string.IsNullOrEmpty(ScreenshotsConfig.Instance.FolderPath)
                ? ScreenshotsConfig.Instance.FolderPath : FolderPathProvider.Get());

            VisualElement textInput = _folderPathField.Q("unity-text-input");
            // Register callback to open folder selection dialog.
            textInput.RegisterCallback<ClickEvent>(e =>
            {
                string folderPath = EditorUtility.OpenFolderPanel("Choose folder", _folderPathField.value, "");

                if (string.IsNullOrEmpty(folderPath))
                    return;

                Undo.RecordObject(ScreenshotsConfig.Instance, "Change Folder Path");

                _folderPathField.value = folderPath;
                ScreenshotsConfig.Instance.FolderPath = folderPath;

                EditorUtility.SetDirty(ScreenshotsConfig.Instance);
                Undo.FlushUndoRecordObjects();
            });
            // Register callback to reveal folder in Finder when middle-clicked.
            textInput.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == 2)
                    EditorUtility.RevealInFinder(_folderPathField.value + Path.DirectorySeparatorChar);
            });

            root.Q<Button>("set-default-btn").clicked += () => _folderPathField.value = FolderPathProvider.Get();
        }

        /// <summary>
        /// Initializes the footer buttons.
        /// </summary>
        /// <param name="root">The root VisualElement of the window.</param>
        private void InitFooter(VisualElement root)
        {
            root.Q<Button>("screenshot-btn").clicked += MakeScreenShot;
            root.Q<Button>("clear-folder-btn").clicked += ClearFolder;
        }

        #endregion

        /// <summary>
        /// Handles key down events. Triggers screenshot capture when Ctrl+E is pressed.
        /// </summary>
        /// <param name="e">The key down event data.</param>
        private void KeyDownHandler(KeyDownEvent e)
        {
            if (e.ctrlKey && e.keyCode == KeyCode.E)
                MakeScreenShot();
        }

        /// <summary>
        /// Captures a screenshot using the selected camera and saves it to the specified folder.
        /// </summary>
        private async void MakeScreenShot()
        {
            Camera camera = _camera ?? SceneView.lastActiveSceneView.camera;
            RenderTexture renderTexture = RenderTextureProvider.Get(ScreenshotsConfig.GetSize());

            RenderTexture oldRT = camera.targetTexture;
            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = oldRT;

            RenderTexture.active = renderTexture;
            Texture2D tex = Texture2DProvider.Get(new(renderTexture.width, renderTexture.height));
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            byte[] bytes = tex.EncodeToPNG();
            string folder = _folderPathField.value;
            string fileName = FileNameProvider.Get() + ".png";
            string path = Path.Combine(folder, fileName);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            await File.WriteAllBytesAsync(path, bytes);

            if (Regex.IsMatch(path, $@"^(Assets|{Application.dataPath})", RegexOptions.IgnoreCase))
                AssetDatabase.ImportAsset(path);

            Debug.Log("Saved to " + path);
        }

        /// <summary>
        /// Clears all files from the specified folder.
        /// </summary>
        private void ClearFolder()
        {
            string folderWithSeparator = _folderPathField.value + Path.DirectorySeparatorChar;
            if (!Directory.Exists(folderWithSeparator))
                return;

            DirectoryInfo di = new DirectoryInfo(folderWithSeparator);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

    }

}
