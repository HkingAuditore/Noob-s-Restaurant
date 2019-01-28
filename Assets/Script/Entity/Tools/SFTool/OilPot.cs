using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class OilPot : Tool {

    Rigidbody potRb;
    Transform potTrans;
    Vector3 potOriLocalPosition;
    Quaternion potOriLocalRotation;
    Vector3 targetPosition;
    Quaternion targetRotation;

    Quaternion pourRot;
    Transform pourAnchorTrans;
    Animator pourOilAnimator;

    bool isPouring;
    bool isInPlace;
    float moveSpeed;

    protected override void Awake()
    {
        potTrans = this.transform.Find("Pot");
        pourAnchorTrans = this.transform.Find("PourAnchor");
        potRb = potTrans.GetComponent<Rigidbody>();
        pourOilAnimator = potTrans.Find("PourOil").GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        isPouring = false;
        isInPlace = true;
        moveSpeed = 2.5f;
        potOriLocalPosition = potTrans.localPosition;
        potOriLocalRotation = potTrans.localRotation;
        targetPosition = potOriLocalPosition;
        targetRotation = potOriLocalRotation;
        pourOilAnimator.gameObject.SetActive(false);
        pourRot = Quaternion.AngleAxis(-45f, Vector3.up);
    }

    public override void OnBeginCtrl()
    {
        base.OnBeginCtrl();

        SetPotRigidbody(false);
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        SetOilPotTargetPos();
        MoveToolToTargetPos(potTrans,targetPosition,targetRotation,moveSpeed,ref isInPlace);
    }

    public override void OnStopCtrl()
    {
        base.OnStopCtrl();

        ResetOilPotState();
        SetPotRigidbody(true);
    }

    void SetPotRigidbody(bool isTrue)
    {
        if (!isTrue)
        {
            potRb.useGravity = false;
            potRb.isKinematic = true;
        }
        else
        {
            potRb.useGravity = true;
            potRb.isKinematic = false;
        }
    }

    void SetOilPotTargetPos()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isPouring = !isPouring;
            isInPlace = false;

            if (isPouring)
            {
                targetPosition = pourAnchorTrans.localPosition;
                targetRotation = pourAnchorTrans.localRotation;
            }
            else
            {
                targetPosition = potOriLocalPosition;
                targetRotation = potOriLocalRotation;
                pourOilAnimator.gameObject.SetActive(false);
                pourOilAnimator.SetBool("isPouring", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isInPlace && isPouring)
            {
                targetRotation = pourAnchorTrans.localRotation * pourRot;
                pourOilAnimator.gameObject.SetActive(true);
                pourOilAnimator.SetBool("isPouring", true);
            }
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            if (isInPlace && isPouring)
            {
                targetRotation = pourAnchorTrans.localRotation;
                pourOilAnimator.gameObject.SetActive(false);
                pourOilAnimator.SetBool("isPouring", false);
            }
        }
    }

    void ResetOilPotState()
    {
        pourOilAnimator.gameObject.SetActive(false);
        pourOilAnimator.SetBool("isPouring", false);
        targetPosition = potOriLocalPosition;
        targetRotation = potOriLocalRotation;
        potTrans.localPosition = potOriLocalPosition;
        potTrans.localRotation = potOriLocalRotation;
        isPouring = false;
        isInPlace = true;
    }
}
