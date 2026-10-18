using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager.UI;
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

        private ActiveFolderPathController _folderPath;
        private FileNameController _fileName;
        private EnumField _fileType;

        private Camera _camera;

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

            InitCamera(root);
            InitOrientation(root, so);
            InitFooter(root);

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
                root.Q<TextField>("file-name__field"),
                root.Q<Button>("file-name__reset-btn")
            );

            _fileType = root.Q<EnumField>("file-name__type");
        }

        /// <summary>
        /// Initializes the camera selection field.
        /// </summary>
        /// <param name="root">The root VisualElement of the window.</param>
        private void InitCamera(VisualElement root)
        {
            ObjectField cameraField = root.Q<ObjectField>("camera-source");
            cameraField.RegisterValueChangedCallback(e => { _camera = e.newValue as Camera; });
        }

        /// <summary>
        /// Initializes the screen orientation field and render texture.
        /// </summary>
        /// <param name="root">The root VisualElement of the window.</param>
        private void InitOrientation(VisualElement root, SerializedObject so)
        {
            var field = root.Q<EnumField>("orientation-field");
            field.BindProperty(so);
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

        /// <summary>
        /// Handles key down events. Triggers screenshot capture when Ctrl+E is pressed.
        /// </summary>
        /// <param name="e">The key down event data.</param>
        private void KeyDownHandler(KeyDownEvent e)
        {
            if (e.ctrlKey && e.keyCode == KeyCode.E)
                MakeScreenShot();
        }

        #endregion


        #region Destroy

        private void OnDestroy()
        {
            _folderPath.Dispose();
        }

        #endregion


        #region Screenshots

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
            string folder = _folderPath.Path;
            string fileName = _fileName.Value + ".png";
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
            //string folderWithSeparator = _folderPath.Path + Path.DirectorySeparatorChar;
            //if (!Directory.Exists(folderWithSeparator))
            //    return;

            //DirectoryInfo di = new DirectoryInfo(folderWithSeparator);

            //foreach (FileInfo file in di.GetFiles())
            //{
            //    file.Delete();
            //}
        }

        #endregion

    }

}
