using DigitalRuby.PyroParticles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SFToolCtrl : ToolCtrl
{
    [SerializeField]
    private Transform fireAnchor;
    [SerializeField]
    private GameObject turner;
    private GameObject firePrefab;
    private bool isFiring;

    private ParticleSystem particleSystemManager;
    private Vector3 oriPosR;
    private Vector3 oriPosL;
    private Rigidbody rb;

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

    private Vector3 lastMousePos;

    void Start()
    {
        particleSystemManager = GameManager.Instance.particleSystem;
        firePrefab = Resources.Load<GameObject>("Prefabs/CampFire");

        oriPosR = turner.transform.position + new Vector3(offsetX, offsetY, offsetZ); ;
        oriPosL = turner.transform.position - Vector3.forward * 1.6f + new Vector3(offsetX, offsetY, offsetZ); ;
        lastMousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        rb = turner.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

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
        turner.transform.position = oriPosR;
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(1))
            is2Right = !is2Right;

        Vector3 delPos = (Input.mousePosition - lastMousePos) * 0.003f;
        Vector3 targetPos = turner.transform.position + new Vector3(-delPos.y, 0, delPos.x);
        Vector3 oriPos = is2Right ? oriPosL : oriPosR;
        turner.transform.position = 
            Vector3.Lerp(turner.transform.position, Vector3.ClampMagnitude(targetPos - oriPos, offsetRadius) + oriPos, 0.8f);
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
                turner.transform.rotation = Quaternion.Slerp(turner.transform.rotation, targetRotL, 0.05f);
            else
                turner.transform.rotation = Quaternion.Slerp(turner.transform.rotation, oriRotL, 0.3f);              
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                //float y = Mathf.Lerp(transform.localPosition.y, targetDownY, 0.3f);
                // transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                turner.transform.rotation = Quaternion.Slerp(turner.transform.rotation, targetRotR, 0.05f);
            }
            else
            {
                //float y = Mathf.Lerp(transform.localPosition.y, oriPos.y, 0.3f);
                //transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                turner.transform.rotation = Quaternion.Slerp(turner.transform.rotation, oriRotR, 0.3f);
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
