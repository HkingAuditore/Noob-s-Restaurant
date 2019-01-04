using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel :IPanel {

    Canvas canvas;
    GameObject loadPanel;
    Text loadText;
    float timer = 0;

    public string GetPanelName()
    {
        return "LoadPanel";
    }

    public void OnEnter()
    {
        //Debug.Log("UI:LoadPanelEnter");     
        canvas = GameObject.Find("Canvas").GetComponentInParent<Canvas>();
        loadPanel = canvas.transform.Find("LoadPanel").gameObject;
        loadText = canvas.transform.Find("LoadPanel/Text").GetComponent<Text>();

        loadPanel.SetActive(true);
    }

    public void OnExit()
    {
        //Debug.Log("UI:LoadPanelExit");
    }

    public void OnPause()
    {
        //Debug.Log("UI:LoadPanelPause");
    }

    public void OnUpdate()
    {
        //Debug.Log("UI:LoadPanelUpdate");
        FlashingLoadText();
    }

    void FlashingLoadText()
    {
        timer+=Time.deltaTime;
        if (timer <= 1)
            loadText.text = "Loading.";
        else if (timer > 1 && timer <= 2)
            loadText.text = "Loading..";
        else if (timer > 2 && timer <= 3)
            loadText.text = "Loading...";
        else
            timer = 0;
    }
}
