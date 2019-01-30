using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTableHintPanel : IPanel {

    Canvas canvas;
    GameObject pTableHintPanel;

    public string GetPanelName()
    {
        return "PTableHintPanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        pTableHintPanel = canvas.transform.Find("PTableHintPanel").gameObject;
        pTableHintPanel.SetActive(true);
    }

    public void OnExit()
    {
        pTableHintPanel.SetActive(false);
    }

    public void OnPause()
    {
        
    }

    public void OnUpdate()
    {
        
    }
}
