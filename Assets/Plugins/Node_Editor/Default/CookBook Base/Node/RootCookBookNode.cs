using UnityEngine;
using NodeEditorFramework.Utilities;
using LitJson;

namespace NodeEditorFramework.Standard {

    [Node(false, "CookBook/Root")]
    public class RootCookBookNode : Node
    {
        public int tempID;
        public const string ID = "rootCookBookNode";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Root"; } }
        public override Vector2 DefaultSize { get { return new Vector2(280, 150); } }

        [ValueConnectionKnob("RootIn", Direction.In, "Float")]
        public ValueConnectionKnob InPort;
        
        public string CookBookName;
        public string Description;

        public override void NodeGUI()
        {
            InPort.DisplayLayout();
            GUILayout.Label("CookBookName:");

            CookBookName = RTEditorGUI.TextField(GUIContent.none, CookBookName);

            GUILayout.Label("Description:");

            Description = RTEditorGUI.TextField(Description, GUILayout.MinWidth(250), GUILayout.MinWidth(300));
        }

        public Node GetInPortNode()
        {
            try
            {
                Node conNode = InPort.connections[0].body;
                return conNode;
            }
            catch (System.Exception e)
            {
                Debug.Log("*Error*<color=red>" + "Tree Find Error ： " + e.Message + "</color>");
                return null;
            }
        }
    }
}