using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    /// <summary>
    /// A custom VisualElement that acts as an inline foldout. 
    /// The first child element remains permanently visible alongside the toggle, while subsequent children are collapsed or expanded.
    /// </summary>
    [UxmlElement(libraryPath = "Screenshots")]
    public partial class InlineFoldout: VisualElement
    {

        #region State

        private static readonly VisualTreeAsset _treeAsset;

        private string _saveKey;

        /// <summary>
        /// Gets the underlying Toggle UI element responsible for expanding and collapsing.
        /// </summary>
        public Toggle Toggle { get; }

        /// <summary>
        /// Overrides the default content container so that added children are correctly nested inside the custom "content-block".
        /// </summary>
        public override VisualElement contentContainer => _contentContainerInstance ?? this;
        private readonly VisualElement _contentContainerInstance;

        /// <summary>
        /// Gets or sets the expanded state of the foldout.
        /// Updating this property automatically aligns the display state of child elements.
        /// </summary>
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

        /// <summary>
        /// Static constructor to load the UXML layout asset only once per domain reload.
        /// </summary>
        static InlineFoldout()
        {
            _treeAsset = AssetDatabase.LoadAssetByGUID<VisualTreeAsset>(new GUID("59061e4c122c4ae46b44cc347cf9ff93"));
        }

        /// <summary>
        /// Initializes a new instance of the foldout, cloning the layout tree and registering necessary event callbacks.
        /// </summary>
        public InlineFoldout()
        {
            _treeAsset.CloneTree(this);

            _contentContainerInstance = this.Q<VisualElement>("content-block");
            _contentContainerInstance.RegisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);
            // Prevents accidental toggling when clicking on elements inside the content container.
            _contentContainerInstance.RegisterCallback<PointerDownEvent>(e => e.StopPropagation());

            Toggle = this.Q<Toggle>();
            Toggle.RegisterValueChangedCallback(OnToggleValueChanged);
        
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Invoked when the user clicks the toggle arrow. Saves the new state and updates child visibility.
        /// </summary>
        /// <param name="evt">The boolean change event.</param>
        private void OnToggleValueChanged(ChangeEvent<bool> evt)
        {
            EditorPrefs.SetBool(_saveKey, evt.newValue);
            AlignChildrenDisplay(evt.newValue);
        }

        /// <summary>
        /// Invoked when the element is added to the active visual tree. 
        /// Restores the previous expanded/collapsed state from EditorPrefs based on the element's name or view data key.
        /// </summary>
        /// <param name="evt">The attach-to-panel event.</param>
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

        /// <summary>
        /// Invoked when the layout or child count of the content container changes. 
        /// Reapplies visibility logic if new elements were added dynamically.
        /// </summary>
        /// <param name="evt">The geometry change event.</param>
        private void OnContentGeometryChanged(GeometryChangedEvent evt)
        {
            if (contentContainer.childCount != _lastChildCount)
            {
                AlignChildrenDisplay(Value);
            }
        }

        #endregion


        #region Activity

        /// <summary>
        /// Adjusts the DisplayStyle of all child elements based on the current toggle state.
        /// The first child is always set to Flex, while the rest are toggled between Flex and None.
        /// </summary>
        /// <param name="activity">True to show all elements, false to hide everything except the first child.</param>
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
