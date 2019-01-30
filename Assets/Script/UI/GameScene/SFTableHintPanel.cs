using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFTableHintPanel : IPanel
{
    Canvas canvas;
    GameObject sfTableHintPanel;

    public string GetPanelName()
    {
        return "SFTableHintPanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        sfTableHintPanel = canvas.transform.Find("SFTableHintPanel").gameObject;
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
