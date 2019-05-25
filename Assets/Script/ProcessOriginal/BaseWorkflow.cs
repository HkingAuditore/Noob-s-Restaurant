using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkTask
{
    public class BaseWorkflow : MonoBehaviour
    {
        public WorkflowNode CurrentNode;

        public virtual void Init() { }

        public virtual void Process() { }

        public virtual void Quit() { }
    }

    public class WorkflowNode : MonoBehaviour
    {
        public bool IsNodeDone;

        public virtual void Init() { }

        public virtual void Quit() { }
    }
}