using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : IState {

    public string GetStateName()
    {
        return Const.SceneStateName.Game.ToString();
    }

    public void OnStateEnd()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnStateStart()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnStateUpdate()
    {
    }
}
