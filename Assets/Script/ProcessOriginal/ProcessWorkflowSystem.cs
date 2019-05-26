using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorkTask
{
    /// <summary>
    /// 线性流
    /// </summary>
    public class ProcessWorkflowSystem : BaseWorkflow
    {
        public Queue<WorkflowNode> NodeQueue = new Queue<WorkflowNode>();

        public override void Init()
        {
            CurrentNode = NodeQueue.Dequeue();
            NodeQueue.TrimExcess();
            CurrentNode.Init();
        }

        public override void Process()
        {
            if (CurrentNode.IsNodeDone)
            {
                if (NodeQueue.Count < 1) { Quit(); return; }
                CurrentNode.Quit();
                CurrentNode = NodeQueue.Dequeue();
                NodeQueue.TrimExcess();
                CurrentNode.Init();
            }
        }

        public override void Quit()
        {
            CurrentNode.Quit();
        }
    }
}