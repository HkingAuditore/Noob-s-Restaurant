using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartScene : IState{

    public string GetStateName()
    {
        return SceneStateName.Start.ToString();
    }

    public bool IsNeedLoadScene()
    {
        return false;
    }

    public void OnStateEnd()
    {
        Debug.Log("scene:StartSceneEnd");
        GameManager.Instance.uiManager.PopPanel();
    }

    public void OnStateStart()
    {
        Debug.Log("scene:StartSceneStart");
        GameManager.Instance.uiManager.PushPanel(new StartPanel());
    }

    public void OnStateUpdate()
    {
        Debug.Log("scene:StartSceneUpdate");
    }  
}
