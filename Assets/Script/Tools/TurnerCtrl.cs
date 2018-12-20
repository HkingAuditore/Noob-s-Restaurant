using DigitalRuby.PyroParticles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnerCtrl : ToolCtrl
{
    public Transform fireAnchor;
    private GameObject firePrefab;
    private bool isFiring;

    private ParticleSystemManager particleSystemManager;
    private Vector3 oriPosR;
    private Vector3 oriPosL;
    private Rigidbody rb;
    public float offsetX = 0.8f;
    public float offsetZ = 0.8f;
    public float downY = 0.5f;
    public float offsetRadius = 0.8f;
    private float targetDownY;
    private bool is2Right = false;

    private Vector3 lastMousePos;

    // Use this for initialization
    void Start()
    {
        particleSystemManager = GameManager.Instance.particleSystemManager;
        firePrefab = Resources.Load<GameObject>("Prefabs/CampFire");

        oriPosR = transform.position;
        oriPosL = transform.position - Vector3.forward * 1.6f;
        lastMousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        targetDownY = oriPosR.y - downY;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCtrlling)
            return;

        Move();
        DoAction();
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

    public void SetPosAtOri()
    {
        transform.position = oriPosR;
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(1))
        {
            is2Right = !is2Right;
        }

        Vector3 delPos = (Input.mousePosition - lastMousePos) * 0.003f;
        //Vector3 pos = new Vector3();
        //pos.z = -delPos.x;
        //pos.y = delPos.y;
        //pos.x = 0;
        Vector3 targetPos = transform.position + new Vector3(-delPos.y, 0, delPos.x);
        //targetPos.x = Mathf.Clamp(targetPos.x, oriPos.x - offsetX, oriPos.x + offsetX);
        //targetPos.z = Mathf.Clamp(targetPos.z, oriPos.z - offsetZ, oriPos.z + offsetZ);
        Vector3 oriPos = is2Right ? oriPosL : oriPosR;
        transform.position = Vector3.Lerp(transform.position, Vector3.ClampMagnitude(targetPos - oriPos, offsetRadius) + oriPos, 0.8f);

        lastMousePos = Input.mousePosition;
    }
    private Quaternion targetRotL = Quaternion.Euler(0f, -95.604f, -12.44f);
    private Quaternion oriRotL = Quaternion.Euler(0f, -95.604f, -50.44f);
    private Quaternion targetRotR = Quaternion.Euler(0f, 95.604f, -12.44f);
    private Quaternion oriRotR = Quaternion.Euler(0f, 95.604f, -50.44f);

    private void DoAction()
    {
        if (is2Right)
        {
            if (Input.GetMouseButton(0))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotL, 0.05f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, oriRotL, 0.3f);
                //        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z- 0.05f);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                //float y = Mathf.Lerp(transform.localPosition.y, targetDownY, 0.3f);
                // transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotR, 0.05f);
            }
            else
            {
                //float y = Mathf.Lerp(transform.localPosition.y, oriPos.y, 0.3f);
                //transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, oriRotR, 0.3f);
                //      transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.05f);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isFiring)
            {
                particleSystemManager.AddFXPrefab(firePrefab, fireAnchor);
                isFiring = true;
            }
            else
            {
                particleSystemManager.StopFXAndRemove(fireAnchor.GetChild(0).gameObject);
                isFiring = false;
            }
        }
    }
}
