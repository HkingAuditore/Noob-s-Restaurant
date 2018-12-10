using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel :IPanel {

    Canvas canvas;
    //bg相关
    Image backGroundImage;
    GameObject startPanel;
    float bgLerpSpeed = 1f;
    Color highValue = Color.white;
    Color lowValue = Color.black;
    Color targetValue;

    public string GetPanelName()
    {
        return "StartPanel";
    }

    public void OnEnter()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        startPanel = canvas.transform.Find("StartPanel").gameObject;
        backGroundImage = canvas.transform.Find("StartPanel/BG").GetComponent<Image>();
        startPanel.SetActive(true); 
        backGroundImage.color = Color.black;
        targetValue = highValue;
    }

    public void OnPause()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        ShowBG();
    }

    void ShowBG()
    {
        backGroundImage.color = Color.Lerp(backGroundImage.color, targetValue, bgLerpSpeed * Time.deltaTime);
        if (Mathf.Abs(backGroundImage.color.r - targetValue.r) < 0.1f)
        {
            if (targetValue == highValue)
            {
                targetValue = lowValue;
            }
            else if (targetValue == lowValue)
            {
                GameManager.Instance.sceneStateManager.SetState(new MenuScene());
            }
        }
    }
}
