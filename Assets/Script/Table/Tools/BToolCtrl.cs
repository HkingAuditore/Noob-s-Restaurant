using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BToolCtrl : ToolCtrl
{
    //private ParticleSystemManager particleSystemManager;

    [SerializeField]
    private GameObject cover;
    [SerializeField]
    private GameObject pot;

    private Vector3 coverOriPos;
    private Rigidbody coverRb;

    [SerializeField]
    private Transform fireAnchor;
    [SerializeField]
    private Transform coverUpAnchor;
    [SerializeField]
    private Transform coverDownAnchor;

    private GameObject firePrefab;
    private bool isFiring;
    private bool isSealing;

    private void Start()
    {
        //particleSystemManager = GameManager.Instance.particleSystemManager;
        firePrefab = Resources.Load<GameObject>("Prefabs/CampFire");
        coverOriPos = cover.transform.position;
        coverRb = cover.GetComponent<Rigidbody>();
        coverRb.useGravity = true;
        coverRb.isKinematic = false;
    }

    private void Update()
    {
        if (!isCtrlling)
            return;

        DoAction();
    }

    protected override void OnBeginCtrl()
    {
        if (!isSealing)
            SetOriPos();
        coverRb.useGravity = false;
        coverRb.isKinematic = true;
    }

    protected override void OnStopCtrl()
    {
        coverRb.useGravity = true;
        coverRb.isKinematic = false;
    }

    private void SetOriPos()
    {
        cover.transform.position = coverOriPos;
    }

    private void DoAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isFiring)
            {
                //particleSystemManager.AddFXPrefab(firePrefab, fireAnchor);
                isFiring = true;
            }
            else
            {
                //particleSystemManager.StopFXAndRemove(fireAnchor.GetChild(0).gameObject);
                isFiring = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
            isSealing = !isSealing;


        if (isSealing)
        {
            cover.transform.position = Vector3.Lerp(cover.transform.position, coverDownAnchor.position, 0.2f);
        }
        else
        {
            cover.transform.position = Vector3.Lerp(cover.transform.position, coverUpAnchor.position, 0.2f);
        }
    }
}
