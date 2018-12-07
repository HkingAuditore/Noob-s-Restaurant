using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState  {
    string GetStateName();
    void OnStateStart();
    void OnStateUpdate();
    void OnStateEnd();
}
