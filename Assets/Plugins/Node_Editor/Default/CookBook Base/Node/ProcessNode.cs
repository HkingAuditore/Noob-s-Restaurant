using UnityEngine;
using NodeEditorFramework.Utilities;

namespace NodeEditorFramework.Standard
{
    

    [Node(false, "CookBook/Process")]
    public class ProcessNode : Node
    {
        public int tempID;
        public const string ID = "processCookBookNode";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Process"; } }
        public override Vector2 DefaultSize { get { return new Vector2(280, 80); } }
        
        [ValueConnectionKnob("in", Direction.In, "Float")]
        public ValueConnectionKnob InputLast;
        [ValueConnectionKnob("out", Direction.Out, "Float")]
        public ValueConnectionKnob OutputNext;

        public enum NodeProcessTag { 腌制, 切丁, 切块, 切条, 打碎 }
        public NodeProcessTag ProcessTag = NodeProcessTag.切丁;

        public override void NodeGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            InputLast.DisplayLayout();

            ProcessTag = (NodeProcessTag)RTEditorGUI.EnumPopup(new GUIContent("Process Type", "The type of Process"), ProcessTag);
            
            OutputNext.DisplayLayout();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public Node GetInPortNode()
        {
            try
            {
                Node conNode = InputLast.connections[0].body;
                return conNode;
            }
            catch (System.Exception e)
            {
                Debug.Log("*Error*<color=red>" + "Tree Find Error ： " + e.Message + "</color>");
                return null;
            }
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

