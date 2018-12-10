using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanel {

    string GetPanelName();
    void OnEnter();
    void OnUpdate();
    void OnPause();
    void OnExit();	
}
