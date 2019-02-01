using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanel : IPanel
{
    Canvas canvas;
    GameObject sfTableHintPanel;

    public string GetPanelName()
    {
        return "EndPanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        sfTableHintPanel = canvas.transform.Find("EndPanel").gameObject;
        sfTableHintPanel.SetActive(true);
    }

    public void OnExit()
    {
        sfTableHintPanel.SetActive(false);
    }

    public void OnPause()
    {

    }

    public void OnUpdate()
    {

    }
}
