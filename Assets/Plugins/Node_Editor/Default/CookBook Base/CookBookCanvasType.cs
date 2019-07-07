using UnityEngine;

namespace NodeEditorFramework.Standard
{
    [NodeCanvasType("CookBook")]
    public class CookBookCanvasType : NodeCanvas
    {

        public override string canvasName { get { return "CookBook"; } }

        private string rootNodeID { get { return "rootCookBookNode"; } }
        public RootCookBookNode rootNode;

        protected override void OnCreate()
        {
            Traversal = new CookBookTraversal(this);
            rootNode = Node.Create(rootNodeID, Vector2.zero, this, null, true) as RootCookBookNode;
        }

        private void OnEnable()
        {
            if (Traversal == null)
                Traversal = new CookBookTraversal(this);
            // Register to other callbacks
            //NodeEditorCallbacks.OnDeleteNode += CheckDeleteNode;
        }

        protected override void ValidateSelf()
        {
            if (Traversal == null)
                Traversal = new CookBookTraversal(this);
            if (rootNode == null && (rootNode = nodes.Find((Node n) => n.GetID == rootNodeID) as RootCookBookNode) == null)
                rootNode = Node.Create(rootNodeID, Vector2.zero, this, null, true) as RootCookBookNode;
        }

        public override bool CanAddNode(string nodeID)
        {
            //Debug.Log ("Check can add node " + nodeID);
            if (nodeID == rootNodeID)
                return !nodes.Exists((Node n) => n.GetID == rootNodeID);
            return true;
        }
    }
}
