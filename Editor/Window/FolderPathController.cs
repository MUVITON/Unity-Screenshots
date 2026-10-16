using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    public class FolderPathController : IDisposable
    {

        private enum Mode
        {
            Default,
            Custom
        }


        #region State

        private readonly EditorSceneManager.SceneOpenedCallback _setDefaultPathByScene;
        private readonly Action<PrefabStage> _setDefaultPathByPrefab;

        private readonly TextField _field;
        private readonly SerializedProperty _property;

        private Mode _mode;

        #endregion


        #region Initialization

        public FolderPathController(TextField field, Button defaultBtn, SerializedProperty property)
        {
            _setDefaultPathByScene  = SetDefaultPathByScene;
            _setDefaultPathByPrefab = SetDefaultPathByPrefab;

            _property = property;

            _field = field;
            _field.RegisterCallback<ClickEvent>(e => SelectFolder());
            _field.RegisterCallback<MouseDownEvent>(e => { if (e.button == 2) OpenFolder(); });
            _field.TrackPropertyValue(property, OnPropertyChanged);

            defaultBtn.RegisterCallback<ClickEvent>(e =>
            {
                _property.stringValue = null;
                _property.serializedObject.ApplyModifiedProperties();
            });

            OnPropertyChanged(_property);
        }

        #endregion


        #region Dispose

        public void Dispose()
        {
            StopTrackSceneChange();
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
            StartTrackSceneChange();
        }

        private void SetCustomMode()
        {
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

        private void SetDefaultPath()
        {
            _field.SetValueWithoutNotify(FolderPathProvider.Get());
        }

        #endregion


        #region Event Handlers

        private void OnPropertyChanged(SerializedProperty prop)
        {
            if (string.IsNullOrEmpty(prop.stringValue))
            {
                SetDefaultPath();
                SetMode(Mode.Default);
            }
            else
            {
                _field.SetValueWithoutNotify(prop.stringValue);
                SetMode(Mode.Custom);
            }
        }

        #endregion


        #region Navigation

        private void SelectFolder()
        {
            string folderPath = EditorUtility.OpenFolderPanel("Choose folder", _field.value, "");

            if (string.IsNullOrEmpty(folderPath))
                return;

            _property.stringValue = folderPath;
            _property.serializedObject.ApplyModifiedProperties();

            SetMode(Mode.Custom);
        }

        private void OpenFolder()
        {
            EditorUtility.RevealInFinder(_field.value + Path.DirectorySeparatorChar);
        }

        #endregion

    }

}
