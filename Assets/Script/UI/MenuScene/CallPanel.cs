using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPanel : MonoBehaviour
{
    public GameObject[] Panels;
    public void CallPanelInList(int target)
    {
        Panels[target].SetActive(true);
    }
}
