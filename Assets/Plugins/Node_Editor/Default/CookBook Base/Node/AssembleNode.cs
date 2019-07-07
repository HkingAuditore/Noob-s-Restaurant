using UnityEngine;
using NodeEditorFramework.Utilities;

namespace NodeEditorFramework.Standard
{

    [Node(false, "CookBook/Assemble")]
    public class AssembleNode : Node
    {
        public int tempID;
        public const string ID = "assembleCookBookNode";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Assemble"; } }
        public override Vector2 DefaultSize { get { return new Vector2(150, 100); } }

        [ValueConnectionKnob("in", Direction.In, "Float")]
        public ValueConnectionKnob Input_1;
        [ValueConnectionKnob("in", Direction.In, "Float")]
        public ValueConnectionKnob Input_2;
        [ValueConnectionKnob("out", Direction.Out, "Float")]
        public ValueConnectionKnob Output;

        public override void NodeGUI()
        {

            Input_1.DisplayLayout();
            Input_2.DisplayLayout();
            

            Output.DisplayLayout();
            
        }

        public Node GetFirstInPortNode()
        {
            try
            {
                Node conNode = Input_1.connection(0).body;
                return conNode;
            }
            catch (System.Exception e)
            {
                Debug.Log("*Error*<color=red>" + "Tree Find Error ： " + e.Message + "</color>");
                return null;
            }
        }
        public Node GetSecondInPortNode()
        {
            try
            {
                Node conNode = Input_2.connection(0).body;
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
                Node conNode = Output.connection(0).body;
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