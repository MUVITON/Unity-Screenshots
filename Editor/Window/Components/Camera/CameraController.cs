using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    [Serializable]
    public sealed class CameraController : IDisposable
    {

        private const string SceneViewClassName = "camera__block--scene-view";

        private VisualElement _container;
        private ObjectField _cameraField;

        [field: SerializeField]
        public Camera Reference { get; private set; }


        public CameraController()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosed += OnSceneClosed;

            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
        }

        public void Dispose()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosed -= OnSceneClosed;

            PrefabStage.prefabStageOpened -= OnPrefabStageOpened;
        }

        public void Bind(VisualElement container)
        {
            _container = container;
            _cameraField = container.Q<ObjectField>("camera-input");

            _cameraField.RegisterValueChangedCallback(e => UpdateCamera(e.newValue as Camera));
            UpdateCamera(Reference);
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (Reference == null)
                UpdateCamera(Reference);
        }

        private void OnSceneClosed(Scene scene)
        {
            if (Reference == null)
                UpdateCamera(Reference);
        }

        private void OnPrefabStageOpened(PrefabStage stage)
        {
            if (Reference?.cameraType != CameraType.SceneView)
                UpdateCamera(SceneView.lastActiveSceneView?.camera);
        }

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
