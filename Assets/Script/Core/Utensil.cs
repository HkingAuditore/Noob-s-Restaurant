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

    protected virtual void Awake()
    {
        DropFoodPos = transform.Find("DropFoodPos").position;
        dropOffset = 1f;
    }

    protected virtual void Update()
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

    public virtual void DoCtrl() { }

    public virtual void OnBeginCtrl() { }

    public virtual void OnStopCtrl() { }
}