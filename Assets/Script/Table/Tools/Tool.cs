using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour, IUsable
{
    protected bool isCtrlling =false;

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

    public virtual void OnBeginCtrl()
    {
        isCtrlling = true;
    }

    public virtual void OnStopCtrl()
    {
        isCtrlling = false;
    }

    public virtual void DoCtrl() { }

}
