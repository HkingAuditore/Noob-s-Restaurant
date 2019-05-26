using Original;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkTask
{
    public enum ProcessMethodType { 切丁 = 0, 切块 = 1, 切条 = 2 }

    public class ProcessOriginalTaskMgr : MonoBehaviour
    {
        ProcessWorkflowSystem _workflowSystem = new ProcessWorkflowSystem();
        public List<WorkflowNode> NodeList = new List<WorkflowNode>();
        public OriginalItemBaseClass[] SelectOriginal;
        public ProcessMethodType ProcessMethod;

        public void Init()
        {
            //注册流节点
            for (int i = 0; i < NodeList.Count; i++)
            {
                _workflowSystem.NodeQueue.Enqueue(NodeList[i]);
            }
            _workflowSystem.Init();
        }

        public void Process()
        {
            Debug.Log("Process");
            _workflowSystem.Process();
        }

        public void Quit()
        {
            _workflowSystem.Quit();
        }
    }
}