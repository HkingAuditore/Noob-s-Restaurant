using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState  {

    string GetStateName();
    bool IsNeedLoadScene();
    void OnStateStart();
    void OnStateUpdate();
    void OnStateEnd();
}
