using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : IState {

    public string GetStateName()
    {
        return SceneStateName.Menu.ToString();
    }

    public void OnStateEnd()
    {
        Debug.Log("Scene:MenuSceneEnd");
        GameManager.Instance.uiManager.PopPanel();
    }

    public void OnStateStart()
    {
        Debug.Log("Scene:MenuSceneStart");
        GameManager.Instance.uiManager.PushPanel(new MainPanel());
    }

    public void OnStateUpdate()
    {
        Debug.Log("Scene:MenuSceneUpdate");
    }
}
