using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitScene : IState
{
    SceneStateManager SceneStateManager;

    public InitScene(SceneStateManager sceneStateManager)
    {
        SceneStateManager = sceneStateManager;
    }

    public string GetStateName()
    {
        return Const.SceneStateName.Init.ToString();
    }

    public void OnStateEnd()
    {
        Debug.Log("Scene:InitSceneEnd");
    }

    public void OnStateStart()
    {
        Debug.Log("Scene:InitSceneStart");
        SceneStateManager.SetState(new StartScene(SceneStateManager));
    }

    public void OnStateUpdate()
    {
        Debug.Log("Scene:InitSceneUpdate");
    }
}
