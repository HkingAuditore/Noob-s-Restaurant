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
        public override Vector2 DefaultSize { get { return new Vector2(240, 80); } }


        [ValueConnectionKnob("in", Direction.Out, "Float")]
        public ValueConnectionKnob OutputNext;

        public enum NodeOriginalType { Tomato = 0, Potato = 1, Egg = 2, Beef = 3 };
        public NodeOriginalType OriginalType = NodeOriginalType.Beef;

        public override void NodeGUI()
        {
            OutputNext.DisplayLayout();
            OriginalType = (NodeOriginalType)RTEditorGUI.EnumPopup(new GUIContent("Original Type", "The type of Original"), OriginalType);
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