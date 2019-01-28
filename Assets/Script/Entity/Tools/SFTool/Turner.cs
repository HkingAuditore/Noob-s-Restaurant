using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turner :Tool {

    private Vector3 oriPosR;
    private Vector3 oriPosL;
    private Vector3 lastMousePos;
    protected Rigidbody rb;

    [SerializeField]
    private float offsetX = 0f;
    [SerializeField]
    private float offsetZ = 0f;
    [SerializeField]
    private float offsetY = 0f;
    [SerializeField]
    private float offsetRadius = 0.8f;
    private float targetDownY;
    private bool is2Right = false;

    private Quaternion targetRotL = Quaternion.Euler(0f, -95.604f, -12.44f);
    private Quaternion oriRotL = Quaternion.Euler(0f, -95.604f, -50.44f);
    private Quaternion targetRotR = Quaternion.Euler(0f, 95.604f, -12.44f);
    private Quaternion oriRotR = Quaternion.Euler(0f, 95.604f, -50.44f);

    protected override void Awake()
    {
        base.Awake();
        rb = this.GetComponent<Rigidbody>();
    }

    protected override void Start ()
    {
        base.Start();

        oriPosR = this.transform.position + new Vector3(offsetX, offsetY, offsetZ); ;
        oriPosL = this.transform.position - Vector3.forward * 1.6f + new Vector3(offsetX, offsetY, offsetZ); ;
        lastMousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        SetRigidbody();
    }

    public override void OnBeginCtrl()
    {
        base.OnBeginCtrl();

        SetPosAtOri();
        SetRigidbody();
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        Move();
        DoAction();
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
            rb.isKinematic = true;//拿起后摆正
        }
        else
        {
            rb.useGravity = true;//当玩家不使用时受重力控制
            rb.isKinematic = false;
        }
    }

    private void SetPosAtOri()
    {
        this.transform.position = oriPosR;
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(1))
            is2Right = !is2Right;

        Vector3 delPos = (Input.mousePosition - lastMousePos) * 0.003f;
        Vector3 targetPos = this.transform.position + new Vector3(-delPos.y, 0, delPos.x);
        Vector3 oriPos = is2Right ? oriPosL : oriPosR;
        this.transform.position =
            Vector3.Lerp(this.transform.position, Vector3.ClampMagnitude(targetPos - oriPos, offsetRadius) + oriPos, 0.8f);
        lastMousePos = Input.mousePosition;
    }

    private void DoAction()
    {

        if (is2Right)
        {
            if (Input.GetMouseButton(0))
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotL, 0.05f);
            else
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, oriRotL, 0.3f);
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                //float y = Mathf.Lerp(transform.localPosition.y, targetDownY, 0.3f);
                // transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotR, 0.05f);
            }
            else
            {
                //float y = Mathf.Lerp(transform.localPosition.y, oriPos.y, 0.3f);
                //transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, oriRotR, 0.3f);
                //      transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.05f);
            }
        }
    }
}
