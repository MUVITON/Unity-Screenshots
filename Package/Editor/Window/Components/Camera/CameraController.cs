using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// Handles the selection and fallback logic for the target camera used in screenshot capture.
    /// Ensures a valid camera is always assigned, falling back to the Scene View or Main Camera if necessary.
    /// </summary>
    [Serializable]
    internal sealed class CameraController : IDisposable
    {

        private const string SceneViewClassName = "camera__block--scene-view";

        private VisualElement _container;
        private ObjectField _cameraField;

        /// <summary>
        /// Gets the currently active camera reference used for rendering the screenshot.
        /// </summary>
        [field: SerializeField]
        public Camera Reference { get; private set; }


        /// <summary>
        /// Initializes a new instance and subscribes to scene management events to track camera availability.
        /// </summary>
        public CameraController()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;

            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
        }

        /// <summary>
        /// Cleans up event subscriptions to prevent memory leaks.
        /// </summary>
        public void Dispose()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosed -= OnSceneClosed;

            PrefabStage.prefabStageOpened -= OnPrefabStageOpened;
        }

        /// <summary>
        /// Binds the controller to the UI layout and initializes the camera field.
        /// </summary>
        /// <param name="container">The root visual element containing the camera UI components.</param>
        public void Bind(VisualElement container)
        {
            _container = container;
            _cameraField = container.Q<ObjectField>("camera-input");

            _cameraField.RegisterValueChangedCallback(e => UpdateCamera(e.newValue as Camera));
            UpdateCamera(Reference);
        }

        /// <summary>
        /// Event handler triggered when a new scene is opened. Refreshes the camera if the current reference is lost.
        /// </summary>
        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (Reference == null)
                UpdateCamera(Reference);
        }

        /// <summary>
        /// Event handler triggered when a scene is closed. Refreshes the camera if the current reference is lost.
        /// </summary>
        private void OnSceneClosed(Scene scene)
        {
            if (Reference == null)
                UpdateCamera(Reference);
        }

        /// <summary>
        /// Event handler triggered when a prefab stage is opened. Forces a switch to the Scene View camera to capture the prefab.
        /// </summary>
        private void OnPrefabStageOpened(PrefabStage stage)
        {
            if (Reference?.cameraType != CameraType.SceneView)
                UpdateCamera(SceneView.lastActiveSceneView?.camera);
        }

        /// <summary>
        /// Attempts to assign a specific camera, updating the UI state based on whether it is a Scene View camera.
        /// </summary>
        /// <param name="camera">The camera to evaluate and set.</param>
        /// <returns>True if the camera was successfully set and is not null.</returns>
        private bool TrySetCamera(Camera camera)
        {
            if (camera == null)
                return false;

            Reference = camera;

            if (Reference.cameraType == CameraType.SceneView)
            {
                _container.AddToClassList(SceneViewClassName);
                _cameraField.SetValueWithoutNotify(null);
            }
            else
            {
                _container.RemoveFromClassList(SceneViewClassName);
                _cameraField.SetValueWithoutNotify(Reference);
            }

            return true;
        }

        /// <summary>
        /// Cascades through available cameras, falling back to Scene View or Main Camera if the requested one is unavailable.
        /// </summary>
        /// <param name="camera">The preferred camera to use.</param>
        private void UpdateCamera(Camera camera)
        {
            if (TrySetCamera(camera))
                return;

            if (TrySetCamera(SceneView.lastActiveSceneView?.camera))
                return;

            foreach (SceneView sceneView in SceneView.sceneViews)
            {
                if (TrySetCamera(sceneView?.camera))
                    return;
            }

            if (TrySetCamera(Camera.main))
            {
                ScreenshotsLogger.Warn("Scene View camera is unavailable. Defaulting to Main Camera.");
                return;
            }

            ScreenshotsLogger.Error("Cannot find any active camera. Please open the Scene View or add a Main Camera to the scene.");
        }
     
    }

}
