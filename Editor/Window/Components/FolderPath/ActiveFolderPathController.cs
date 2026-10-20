using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Controls the active destination folder path for saving screenshots.
    /// Supports toggling between an automatic mode (dynamically appending the scene or prefab name) 
    /// and a custom user-defined path mode.
    /// </summary>
    internal sealed class ActiveFolderPathController : FolderPathController, IDisposable
    {

        /// <summary>
        /// Defines the operational state of the active folder path.
        /// </summary>
        private enum Mode
        {
            /// <summary>
            /// Uninitialized or undefined state.
            /// </summary>
            None,

            /// <summary>
            /// Automatically generates the path based on the default folder and current scene/prefab.
            /// </summary>
            Default,

            /// <summary>
            /// Uses a specific, persistent path manually chosen by the user.
            /// </summary>
            Custom
        }


        #region State

        private readonly EditorSceneManager.SceneOpenedCallback _setDefaultPathByScene;
        private readonly Action<PrefabStage> _setDefaultPathByPrefab;

        private readonly Button _autoModeBtn;

        private SerializedProperty _defaultPath;

        private Mode _mode;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes the controller, binds the auto-mode toggle button, and sets up path tracking.
        /// </summary>
        /// <param name="container">The root visual element containing the folder UI controls.</param>
        /// <param name="activePath">The serialized property storing the custom path (if any).</param>
        /// <param name="defaultPath">The serialized property storing the base default directory.</param>
        public ActiveFolderPathController(VisualElement container, SerializedProperty activePath, SerializedProperty defaultPath)
            : base(container, activePath)
        {
            // Caching delegates to ensure safe event unsubscription later.
            _setDefaultPathByScene  = SetDefaultPathByScene;
            _setDefaultPathByPrefab = SetDefaultPathByPrefab;

            _defaultPath = defaultPath;

            // When the base default path changes, update the active path UI if we are in auto mode.
            Input.TrackPropertyValue(_defaultPath, p => OnPropertyChanged(Property));

            _autoModeBtn = container.Q<Button>("auto-mode-btn");
            _autoModeBtn.RegisterCallback<ClickEvent>(e =>
            {
                if (_mode == Mode.Default)
                    Property.stringValue = Path; // Lock in the current auto-generated path as a custom path.
                else
                    Property.stringValue = null; // Clear the custom path to revert to auto-generation mode.

                Property.serializedObject.ApplyModifiedProperties();
            });

            OnPropertyChanged(Property);
        }

        #endregion


        #region Mode

        /// <summary>
        /// Safely transitions the controller to a new mode if it differs from the current one.
        /// </summary>
        /// <param name="mode">The target mode to apply.</param>
        private void SetMode(Mode mode)
        {
            if (_mode != mode)
                ApplyMode(mode);
        }

        /// <summary>
        /// Applies the visual and logical changes required for the requested mode.
        /// </summary>
        /// <param name="mode">The mode to activate.</param>
        private void ApplyMode(Mode mode)
        {
            _mode = mode;

            if (_mode == Mode.Default)
                SetDefaultMode();
            else
                SetCustomMode();
        }

        /// <summary>
        /// Activates automatic mode, updates UI button state, and starts listening for scene changes.
        /// </summary>
        private void SetDefaultMode()
        {
            IconButtonSelectionController.ApplyState(_autoModeBtn, true);
            StartTrackSceneChange();
        }

        /// <summary>
        /// Activates custom mode, disables the auto button highlight, and stops listening to scene changes.
        /// </summary>
        private void SetCustomMode()
        {
            IconButtonSelectionController.ApplyState(_autoModeBtn, false);
            StopTrackSceneChange();
        }

        #endregion


        #region Scene Tracking

        /// <summary>
        /// Subscribes to Unity editor events to track when scenes or prefabs are opened and closed.
        /// </summary>
        private void StartTrackSceneChange()
        {
            EditorSceneManager.sceneOpened += _setDefaultPathByScene;
            PrefabStage.prefabStageOpened  += _setDefaultPathByPrefab;
            PrefabStage.prefabStageClosing += _setDefaultPathByPrefab;
        }

        /// <summary>
        /// Unsubscribes from Unity editor events to halt path auto-generation updates.
        /// </summary>
        private void StopTrackSceneChange()
        {
            EditorSceneManager.sceneOpened -= _setDefaultPathByScene;
            PrefabStage.prefabStageOpened  -= _setDefaultPathByPrefab;
            PrefabStage.prefabStageClosing -= _setDefaultPathByPrefab;
        }

        /// <summary>
        /// Event handler triggered when a new scene is opened in the editor.
        /// </summary>
        private void SetDefaultPathByScene(Scene scene, OpenSceneMode mode)
        {
            SetDefaultPath();
        }

        /// <summary>
        /// Event handler triggered when a prefab stage is opened or closed.
        /// </summary>
        private void SetDefaultPathByPrefab(PrefabStage stage)
        {
            SetDefaultPath();
        }

        #endregion


        #region Update Handlers

        /// <summary>
        /// Triggered whenever the active path serialized property changes.
        /// Evaluates whether to switch between Default (auto) and Custom modes.
        /// </summary>
        /// <param name="property">The serialized property representing the active custom path.</param>
        protected override void OnPropertyChanged(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.stringValue))
            {
                SetDefaultPath();
                SetMode(Mode.Default);
            }
            else
            {
                Input.SetValueWithoutNotify(property.stringValue);
                SetMode(Mode.Custom);
            }
        }

        /// <summary>
        /// Generates a directory path by combining the base default folder with the active scene or prefab name.
        /// </summary>
        private void SetDefaultPath()
        {
            string name = null;

            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

            if (stage != null)
            {
                name = stage.scene.name;
            } 
            else
            {
                Scene scene = SceneManager.GetActiveScene();
                name = scene.name;
            }

            if (string.IsNullOrEmpty(name))
                name = "Untitled";

            Input.SetValueWithoutNotify($"{_defaultPath.stringValue}/{name}");
        }

        #endregion


        #region Dispose

        /// <summary>
        /// Cleans up event subscriptions when the controller is destroyed or the window is closed.
        /// </summary>
        public void Dispose()
        {
            StopTrackSceneChange();
        }

        #endregion

    }

}
