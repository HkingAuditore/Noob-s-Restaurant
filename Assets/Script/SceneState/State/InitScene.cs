using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitScene : IState
{
    public string GetStateName()
    {
        return SceneStateName.Init.ToString();
    }

    public void OnStateEnd()
    {
        Debug.Log("Scene:InitSceneEnd");
    }

    public void OnStateStart()
    {
        Debug.Log("Scene:InitSceneStart");
       GameManager.Instance.sceneStateManager.SetState(new StartScene());
    }

    public void OnStateUpdate()
    {
        Debug.Log("Scene:InitSceneUpdate");
    }
}
