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
    }

    public void OnStateUpdate()
    {
        Debug.Log("Scene:MenuSceneUpdate");
    }

}
