using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotCtrl : ToolCtrl
{
    private ParticleSystemManager particleSystemManager;

    public GameObject cover;
    private Vector3 coverOriPos;
    private Rigidbody coverRb;
    public Transform fireAnchor;
    public Transform coverUpAnchor;
    public Transform coverDownAnchor;
    private GameObject firePrefab;
    private bool isFiring;
    private bool isSealing;

    private void Start()
    {
        particleSystemManager = GameManager.Instance.particleSystemManager;
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
                //fire.GetComponent<FireConstantBaseScript>().Stop();
                //Array.ForEach(fireAnchor.GetComponentsInChildren<ParticleSystem>(), (ps) => ps.Stop());
                //fireAnchor.SetActive(false);
                particleSystemManager.AddFXPrefab(firePrefab, fireAnchor);
                isFiring = true;
            }
            else
            {
                //fireAnchor.SetActive(true);
                //Array.ForEach(fireAnchor.GetComponentsInChildren<ParticleSystem>(), (ps) => ps.Play());
                particleSystemManager.StopFXAndRemove(fireAnchor.GetChild(0).gameObject);
                isFiring = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
            isSealing = !isSealing;


        if (isSealing)
        {
            //if (Vector3.Distance(cover.transform.position, coverDownAnchor.position) > 0.1f)
            cover.transform.position = Vector3.Lerp(cover.transform.position, coverDownAnchor.position, 0.2f);
            //else
            //    isSealed = true;
        }
        else
        {
            //if (Vector3.Distance(cover.transform.position, coverUpAnchor.position) > 0.1f)
            cover.transform.position = Vector3.Lerp(cover.transform.position, coverUpAnchor.position, 0.2f);
            //else
            //    isSealed = false;
        }
    }
}
