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
        throw new System.NotImplementedException();
    }

    public void OnStateStart()
    {
        throw new System.NotImplementedException();
    }

    public void OnStateUpdate()
    {
        throw new System.NotImplementedException();
    }
}
