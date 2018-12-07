using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : IState {

    SceneStateManager sceneStateManager;

    public MenuScene(SceneStateManager sceneStateManager)
    {
        this.sceneStateManager = sceneStateManager;
    }

    public string GetStateName()
    {
        return Const.SceneStateName.Menu.ToString();
    }

    public void OnStateEnd()
    {
        Debug.Log("Scene:MenuSceneEnd");
    }

    public void OnStateStart()
    {
        Debug.Log("Scene:MenuSceneStart");
        GameManager.Instance.UIManager.Init();
        GameManager.Instance.UIManager.PushPanel("MainPanel");
    }

    public void OnStateUpdate()
    {
    }
}
