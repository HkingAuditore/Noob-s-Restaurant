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

    public bool IsNeedLoadScene()
    {
        return false;
    }

    public void OnStateEnd()
    {
    }

    public void OnStateStart()
    {
       GameManager.Instance.sceneStateManager.SetState(new StartScene());
    }

    public void OnStateUpdate()
    {
    }
}
