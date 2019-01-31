using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : IState
{

    public string GetStateName()
    {
        return SceneStateName.Game.ToString();
    }

    public bool IsNeedLoadScene()
    {
        return true;
    }

    public void OnStateEnd()
    {
        Debug.Log("Scene:GameSceneEnd");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnStateStart()
    {
        Debug.Log("Scene:GameSceneStart");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        
        GameManager.Instance.sequenceManager.StartSequence("StiredEggAndTomato");
    }

    public void OnStateUpdate()
    {
        Debug.Log("Scene:GameSceneUpdate");
    }
}
