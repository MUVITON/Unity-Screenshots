using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public sealed class ActiveFolderPathController : FolderPathController, IDisposable
    {

        private enum Mode
        {
            None,
            Default,
            Custom
        }


        #region State

        private readonly EditorSceneManager.SceneOpenedCallback _setDefaultPathByScene;
        private readonly Action<PrefabStage> _setDefaultPathByPrefab;

        private SerializedProperty _defaultPath;

        private Mode _mode;

        #endregion


        #region Initialization

        public ActiveFolderPathController(FolderField field, SerializedProperty activePath, SerializedProperty defaultPath)
            : base(field, activePath)
        {
            _setDefaultPathByScene  = SetDefaultPathByScene;
            _setDefaultPathByPrefab = SetDefaultPathByPrefab;

            _defaultPath = defaultPath;

            Field.Input.TrackPropertyValue(_defaultPath, p => OnPropertyChanged(Property));

            Field.SecondButton.RegisterCallback<ClickEvent>(e =>
            {
                if (_mode == Mode.Default)
                    Property.stringValue = Path;
                else
                    Property.stringValue = null;

                Property.serializedObject.ApplyModifiedProperties();
            });

            OnPropertyChanged(Property);
        }

        #endregion


        #region Mode

        private void SetMode(Mode mode)
        {
            if (_mode != mode)
                ApplyMode(mode);
        }

        private void ApplyMode(Mode mode)
        {
            _mode = mode;

            if (_mode == Mode.Default)
                SetDefaultMode();
            else
                SetCustomMode();
        }

        private void SetDefaultMode()
        {
            IconButtonSelectionController.ApplyState(Field.SecondButton, true);
            StartTrackSceneChange();
        }

        private void SetCustomMode()
        {
            IconButtonSelectionController.ApplyState(Field.SecondButton, false);
            StopTrackSceneChange();
        }

        #endregion


        #region Scene Tracking

        private void StartTrackSceneChange()
        {
            EditorSceneManager.sceneOpened += _setDefaultPathByScene;
            PrefabStage.prefabStageOpened  += _setDefaultPathByPrefab;
        }

        private void StopTrackSceneChange()
        {
            EditorSceneManager.sceneOpened -= _setDefaultPathByScene;
            PrefabStage.prefabStageOpened  -= _setDefaultPathByPrefab;
        }

        private void SetDefaultPathByScene(Scene scene, OpenSceneMode mode)
        {
            SetDefaultPath();
        }

        private void SetDefaultPathByPrefab(PrefabStage stage)
        {
            SetDefaultPath();
        }

        #endregion


        #region Update Handlers

        protected override void OnPropertyChanged(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(property.stringValue))
            {
                SetDefaultPath();
                SetMode(Mode.Default);
            }
            else
            {
                Field.Input.SetValueWithoutNotify(property.stringValue);
                SetMode(Mode.Custom);
            }
        }
        private void SetDefaultPath()
        {
            Scene scene = SceneManager.GetActiveScene();
            Field.Input.SetValueWithoutNotify($"{_defaultPath.stringValue}/{scene.name}");
        }

        #endregion


        #region Dispose

        public void Dispose()
        {
            StopTrackSceneChange();
        }

        #endregion

    }

}
