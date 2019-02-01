using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanelButton : MonoBehaviour
{
    public void GoToTitileScene()
    {
        GameManager.Instance.sceneStateManager.SetState(new MenuScene());
    }
}
