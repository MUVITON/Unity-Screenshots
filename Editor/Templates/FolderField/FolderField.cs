using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace Screenshots.Editor
{

    [UxmlElement(libraryPath = "Screenshots")]
    public partial class FolderField : VisualElement
    {

        private static readonly VisualTreeAsset _treeAsset;


        public TextField Input { get; }
        public Button FirstButton { get; }
        public Button SecondButton { get; }


        [UxmlAttribute]
        public string Label 
        { 
            get => Input.label; 
            set => Input.label = value; 
        }

        [UxmlAttribute]
        public string PlaceholderText
        {
            get => Input.textEdition.placeholder;
            set => Input.textEdition.placeholder = value;
        }

        [UxmlAttribute]
        public string FirstButtonText
        {
            get => FirstButton.text;
            set => FirstButton.text = value;
        }

        [UxmlAttribute]
        public string SecondButtonText
        {
            get => SecondButton.text;
            set => SecondButton.text = value;
        }


        static FolderField()
        {
            _treeAsset = AssetDatabase.LoadAssetByGUID<VisualTreeAsset>(new GUID("405b6e2b57ab6ca44b846176d0595190"));
        }

        public FolderField()
        {
            _treeAsset.CloneTree(this);

            Input        = this.Q<TextField>();
            FirstButton  = this.Q<Button>("first-btn");
            SecondButton = this.Q<Button>("second-btn");
        }

    }

}
