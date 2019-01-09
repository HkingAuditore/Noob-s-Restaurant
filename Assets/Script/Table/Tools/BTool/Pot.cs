using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : Tool {

    private GameObject cover;
    private Vector3 coverOriPos;
    private Rigidbody rb;
    [SerializeField]
    private Transform coverUpAnchor;
    [SerializeField]
    private Transform coverDownAnchor;
    private bool isSealing;

    private void Awake()
    {
        cover = this.transform.Find("Pot_Cover").gameObject;
        rb = cover.GetComponent<Rigidbody>();
        coverOriPos = cover.transform.position;
    }

    private void Start()
    {
        SetRigidbody();
    }

    public override void OnBeginCtrl()
    {
        base.OnBeginCtrl();

        SetCoverOriPos();
        SetRigidbody();
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

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
            rb.isKinematic = true;
        }
        else
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    private void SetCoverOriPos()
    {
        if (!isSealing)
            cover.transform.position = coverOriPos;
    }

    private void DoAction()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isSealing = !isSealing;
            Debug.Log(isSealing);
        }

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
