namespace NodeEditorFramework.Standard
{

    public class CookBookTraversal : NodeCanvasTraversal
    {

        CookBookCanvasType Canvas;

        public CookBookTraversal(CookBookCanvasType canvas) : base(canvas)
        {
            Canvas = canvas;
        }

        /// <summary>
        /// Traverse the canvas and evaluate it
        /// </summary>
        public override void TraverseAll()
        {
            RootCookBookNode rootNode = Canvas.rootNode;
            rootNode.Calculate();
            //Debug.Log ("RootNode is " + rootNode);
        }
    }
}
