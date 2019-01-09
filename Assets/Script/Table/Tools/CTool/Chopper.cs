﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chopper : Tool
{
    private Vector3 oriPos;
    private Rigidbody rb;
    [SerializeField]
    private float offsetY = 0.3f;
    [SerializeField]
    private float offsetZ = 0.8f;
    private Vector3 lastMousePos;

    void Start()
    {
        oriPos = this.transform.localPosition;
        rb = this.GetComponent<Rigidbody>();
        lastMousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        SetRigidbody();
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        Move();
    }

    public override void OnBeginCtrl()
    {
        base.OnBeginCtrl();

        SetPosAtOri();
        SetRigidbody();
        Debug.Log("enter");
    }

    public override void OnStopCtrl()
    {
        base.OnStopCtrl();

        SetRigidbody();
    }

    private void SetRigidbody()
    { 
        if (isCtrlling)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        else
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    public void SetPosAtOri()
    {
        this.transform.localPosition = oriPos;
    }

    private void Move()
    {
        Vector3 delPos = (Input.mousePosition - lastMousePos) * 0.005f;
        Vector3 targetPos = this.transform.localPosition + new Vector3(0, delPos.y, -delPos.x);

        targetPos.y = Mathf.Clamp(targetPos.y, oriPos.y - offsetY, oriPos.y + offsetY);
        targetPos.z = Mathf.Clamp(targetPos.z, oriPos.z - offsetZ, oriPos.z + offsetZ);
        this.transform.localPosition = targetPos;
        this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        lastMousePos = Input.mousePosition;
    }

}
