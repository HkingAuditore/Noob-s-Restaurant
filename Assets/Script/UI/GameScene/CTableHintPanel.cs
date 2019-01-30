using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTableHintPanel : IPanel {

    Canvas canvas;
    GameObject cTableHintPanel;

    public string GetPanelName()
    {
        return "CTableHintPanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        cTableHintPanel = canvas.transform.Find("CTableHintPanel").gameObject;
        cTableHintPanel.SetActive(true);
    }

    public void OnExit()
    {
        cTableHintPanel.SetActive(false);
    }

    public void OnPause()
    {

    }

    public void OnUpdate()
    {

    }
}
