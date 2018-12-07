using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : IState{

    SceneStateManager sceneStateManager;
    Canvas canvas;

    //bg相关
    Image backGroundImage;
    float bgLerpSpeed = 1f;
    Color highValue = Color.white;
    Color lowValue = Color.black;
    Color targetValue;

    public StartScene(SceneStateManager sceneStateManager)
    {
        this.sceneStateManager = sceneStateManager;
    }

    public string GetStateName()
    {
        return Const.SceneStateName.Start.ToString();
    }

    public void OnStateEnd()
    {
        Debug.Log("scene:StartSceneEnd");
    }

    public void OnStateStart()
    {
        Debug.Log("scene:StartSceneStart");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        backGroundImage = canvas.transform.Find("BG").GetComponent<Image>();
        backGroundImage.color = Color.black;
        targetValue = highValue;
    }

    public void OnStateUpdate()
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
                sceneStateManager.SetState(new MenuScene(sceneStateManager));
            }
        }
    }
}
