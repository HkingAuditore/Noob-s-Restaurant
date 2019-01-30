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

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

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

    protected virtual void SwitchTool() { }

    protected void MoveToolToTargetPos(Transform toolTrans,Vector3 targetPosition,Quaternion targetRotation,float moveSpeed,ref bool isInPlace)
    {
        if (Vector3.Magnitude(toolTrans.localPosition - targetPosition) > 0.1f ||
    Quaternion.Angle(toolTrans.localRotation, targetRotation) > 0.1f)
        {
            Debug.Log("osa");
            toolTrans.localPosition = Vector3.Lerp(toolTrans.localPosition, targetPosition, Time.deltaTime * moveSpeed);
            toolTrans.localRotation = Quaternion.Lerp(toolTrans.localRotation, targetRotation, Time.deltaTime * moveSpeed);
        }
        else
        {
            if (!isInPlace)
                isInPlace = true;
        }
    }
}
