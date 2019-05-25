using Original;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkTask;

public class SelectProcessOriginalNode : WorkflowNode
{
    public GameObject SelectOriginalPanelPrefab;
    public ProcessOriginalTaskMgr taskMgr;

    public override void Init()
    {
        this.gameObject.SetActive(true);
        SelectOriginalPanelPrefab.SetActive(true);
        SelectOriginalPanelPrefab.GetComponent<OriginalSelectController>().OpenSelectPanel();
        SelectOriginalPanelPrefab.GetComponent<OriginalSelectController>().SelectDoneEventHandler += SelectDone;
    }

    public override void Quit()
    {
        this.gameObject.SetActive(false);
        SelectOriginalPanelPrefab.SetActive(false);
    }

    public void SelectDone(OriginalItemBaseClass[] target) {
        taskMgr.SelectOriginal = target;
        IsNodeDone = true;
        taskMgr.Process();
    }
}
