using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CToolCtrl : ToolCtrl
{
    [SerializeField]
    private GameObject chopper;
    private Vector3 oriPos;
    private Rigidbody rb;
    [SerializeField]
    private float offsetY = 0.3f;
    [SerializeField]
    private float offsetZ = 0.8f;

    private Vector3 lastMousePos;

    // Use this for initialization
    void Start()
    {
        oriPos = chopper.transform.localPosition;
        rb = chopper.GetComponent<Rigidbody>();

        lastMousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        rb.useGravity = true;
        rb.isKinematic = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isCtrlling)
            return;

        chopper.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Move();
    }

    protected override void OnBeginCtrl()
    {
        SetPosAtOri();
        rb.useGravity = false;
        rb.isKinematic = true;//拿起后摆正
    }
    protected override void OnStopCtrl()
    {
        rb.useGravity = true;//当玩家不使用时受重力控制
        rb.isKinematic = false;
    }

    private void Move()
    {
        Vector3 delPos = (Input.mousePosition - lastMousePos) * 0.005f;
        //Vector3 pos = new Vector3();
        //pos.z = -delPos.x;
        //pos.y = delPos.y;
        //pos.x = 0;
        Vector3 targetPos = chopper.transform.localPosition + new Vector3(0, delPos.y, -delPos.x);


        targetPos.y = Mathf.Clamp(targetPos.y, oriPos.y - offsetY, oriPos.y + offsetY);
        targetPos.z = Mathf.Clamp(targetPos.z, oriPos.z - offsetZ, oriPos.z + offsetZ);
        chopper.transform.localPosition = targetPos;

        lastMousePos = Input.mousePosition;
    }

    public void SetPosAtOri()
    {
        chopper.transform.localPosition = oriPos;
    }
}
