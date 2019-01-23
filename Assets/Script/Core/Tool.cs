using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手动控制的工具
/// </summary>
public abstract class Tool : MonoBehaviour, IUsable
{
    protected bool isCtrlling = false;

    public bool IsCtrlling
    {
        get
        {
            return isCtrlling;
        }

        set
        {
            isCtrlling = value;
        }
    }

    private void Update()
    {
        if (!isCtrlling)
            return;

        DoCtrl();
    }

    public void BeginCtrl()
    {
        isCtrlling = true;
        OnBeginCtrl();
    }

    public void StopCtrl()
    {
        isCtrlling = false;
        OnStopCtrl();
    }

    public virtual void OnBeginCtrl() { }

    public virtual void OnStopCtrl() { }

    public virtual void DoCtrl() { }

}
