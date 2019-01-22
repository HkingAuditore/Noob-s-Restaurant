using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 烹饪、食材处理用具
/// </summary>
public abstract class Utensil : Container, IUsable
{
    public bool isCtrlling = false;
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

    public virtual void DoCtrl() { }

    public virtual void OnBeginCtrl() { }

    public virtual void OnStopCtrl() { }
}