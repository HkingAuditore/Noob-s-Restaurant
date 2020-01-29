using UnityEngine;

using NodeEditorFramework.Utilities;

namespace NodeEditorFramework.Standard
{
    [Node(false, "CookBook/Original")]
    public class OriginalNode : Node
    {
        public int tempID;
        public const string ID = "originalCookBookNode";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Original"; } }
        public override Vector2 DefaultSize { get { return new Vector2(240, 240); } }


        [ValueConnectionKnob("in", Direction.Out, "Float")]
        public ValueConnectionKnob OutputNext;

        public enum NodeOriginalType { Tomato = 0, Potato = 1, Egg = 2, Beef = 3 };
        public NodeOriginalType OriginalType = NodeOriginalType.Beef;
        public Mesh OriginalModel;
        public Material OriginalMaterial;

        public override void NodeGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            OutputNext.DisplayLayout();
            OriginalType = (NodeOriginalType)RTEditorGUI.EnumPopup(new GUIContent("Original Type", "The type of Original"), OriginalType);
            GUILayout.Label("Specific Property:");
            GUILayout.Label("Model:");
            OriginalModel = (Mesh)RTEditorGUI.ObjectField(OriginalModel,false);
            GUILayout.Label("Material:");
            OriginalMaterial = (Material)RTEditorGUI.ObjectField(OriginalMaterial,false);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public Node GetOutPortNode()
        {
            try
            {
                Node conNode = OutputNext.connections[0].body;
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