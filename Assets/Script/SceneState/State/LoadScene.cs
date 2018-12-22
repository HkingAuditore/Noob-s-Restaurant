using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : IState
{
    public string GetStateName()
    {
        return SceneStateName.Load.ToString();
    }

    public void OnStateEnd()
    {
        Debug.Log("Scene:LoadSceneEnd");
        GameManager.Instance.uiManager.PopPanel();
    }

    public void OnStateStart()
    {
        Debug.Log("Scene:LoadSceneStart");
        GameManager.Instance.uiManager.PushPanel(new LoadPanel());
    }

    public void OnStateUpdate()
    {
        Debug.Log("Scene:LoadSceneUpdate");
    }
}
