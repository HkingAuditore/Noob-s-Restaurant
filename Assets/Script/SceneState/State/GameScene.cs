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
        
    }

    public void OnStateStart()
    {
        
    }

    public void OnStateUpdate()
    {
        
    }
}
