using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : IState
{
    SceneStateManager SceneStateManager;

    public LoadScene(SceneStateManager sceneStateManager)
    {
        SceneStateManager = sceneStateManager;
    }

    public string GetStateName()
    {
        return Const.SceneStateName.Load.ToString();
    }

    public void OnStateEnd()
    {
      
    }

    public void OnStateStart()
    {

    }

    public void OnStateUpdate()
    {
        
    }
}
