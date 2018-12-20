using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolCtrl : MonoBehaviour
{
    public bool isCtrlling = false;

    public void BeginCtrl()
    {
        OnBeginCtrl();
        isCtrlling = true;
    }

    public void StopCtrl()
    {
        isCtrlling = false;
        OnStopCtrl();
    }

    protected virtual void OnBeginCtrl()
    {

    }

    protected virtual void OnStopCtrl()
    {

    }
}
