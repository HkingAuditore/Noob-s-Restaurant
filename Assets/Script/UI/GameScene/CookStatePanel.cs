using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookStatePanel : IPanel {

    Canvas canvas;
    GameObject cookStatePanel;

    public string GetPanelName()
    {
        return "CookStatePanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        cookStatePanel = canvas.transform.Find("CookStatePanel").gameObject;
        cookStatePanel.SetActive(true);
    }

    public void OnExit()
    {
        cookStatePanel.SetActive(false);
    }

    public void OnPause()
    {

    }

    public void OnUpdate()
    {

    }
}
