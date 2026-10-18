using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    [UxmlElement(libraryPath = "Screenshots")]
    public partial class InlineFoldout: VisualElement
    {

        #region State

        private static readonly VisualTreeAsset _treeAsset;


        private string _saveKey;

        public Toggle Toggle { get; }

        public override VisualElement contentContainer => _contentContainerInstance ?? this;
        private readonly VisualElement _contentContainerInstance;


        [UxmlAttribute]
        public bool Value
        {
            get => Toggle.value;
            set
            {
                Toggle.SetValueWithoutNotify(value);
                AlignChildrenDisplay(value);
            }
        }

        private int _lastChildCount = -1;

        #endregion


        #region Initialization

        static InlineFoldout()
        {
            _treeAsset = AssetDatabase.LoadAssetByGUID<VisualTreeAsset>(new GUID("59061e4c122c4ae46b44cc347cf9ff93"));
        }

        public InlineFoldout()
        {
            _treeAsset.CloneTree(this);

            _contentContainerInstance = this.Q<VisualElement>("content-block");
            _contentContainerInstance.RegisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);

            Toggle = this.Q<Toggle>();
            Toggle.RegisterValueChangedCallback(OnToggleValueChanged);
        
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        #endregion


        #region Event Handlers

        private void OnToggleValueChanged(ChangeEvent<bool> evt)
        {
            EditorPrefs.SetBool(_saveKey, evt.newValue);
            AlignChildrenDisplay(evt.newValue);
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            string id = !string.IsNullOrEmpty(viewDataKey) ? viewDataKey : name;
            _saveKey = $"Screenshots_UI_State_{id}";

            if (EditorPrefs.HasKey(_saveKey))
            {
                Toggle.SetValueWithoutNotify(EditorPrefs.GetBool(_saveKey));
            }

            AlignChildrenDisplay(Value);
        }

        private void OnContentGeometryChanged(GeometryChangedEvent evt)
        {
            if (contentContainer.childCount != _lastChildCount)
            {
                AlignChildrenDisplay(Value);
            }
        }

        #endregion


        #region Activity

        private void AlignChildrenDisplay(bool activity)
        {
            _lastChildCount = contentContainer.childCount;

            if (_lastChildCount > 0)
                contentContainer[0].style.display = DisplayStyle.Flex;

            DisplayStyle display = activity ? DisplayStyle.Flex : DisplayStyle.None;
            for (int i = 1; i < _lastChildCount; i++)
            {
                contentContainer[i].style.display = display;
            }
        }

        #endregion

    }

}
