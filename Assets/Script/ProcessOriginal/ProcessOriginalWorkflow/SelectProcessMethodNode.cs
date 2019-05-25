using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorkTask;

public class SelectProcessMethodNode : WorkflowNode
{
    public GameObject SelectProcessMethodUICanvas;
    public ProcessOriginalTaskMgr taskMgr;
    public Toggle[] Toggles;

    public override void Init()
    {
        this.gameObject.SetActive(true);
        SelectProcessMethodUICanvas.SetActive(true);

        IsNodeDone = true;
    }

    public override void Quit()
    {

        for (int i = 0; i < Toggles.Length; i++)
        {
            if (Toggles[i].isOn) taskMgr.ProcessMethod = (ProcessMethodType)i;
        }

        Debug.Log("MethodProcess");
        this.gameObject.SetActive(false);
        SelectProcessMethodUICanvas.SetActive(false);
    }
}
