using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class OilPot : Tool {

    Rigidbody potRb;
    Transform potTrans;
    Vector3 potOriLocalPosition;
    Vector3 targetPosition;
    Quaternion targetRotation;
    Quaternion potOriLocalRotation;
    Quaternion pourRot;
    Transform pourAnchor;
    Animator pourOilAnimator;

    bool isPouring;
    bool isInPlace;
    float moveSpeed;

    private void Awake()
    {
        potTrans = this.transform.Find("Pot");
        pourAnchor = this.transform.Find("PourAnchor");
        potRb = potTrans.GetComponent<Rigidbody>();
        pourOilAnimator = potTrans.Find("PourOil").GetComponent<Animator>();
    }

    private void Start()
    {
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

        SetPotRigidbody();
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        PourOil();
    }

    public override void OnStopCtrl()
    {
        base.OnStopCtrl();

        ResetOilPotState();
        SetPourOilAnim(false);
        SetPotRigidbody();
    }

    private void SetPotRigidbody()
    {
        if (isCtrlling)
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

    private void PourOil()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isPouring = !isPouring;
            isInPlace = false;

            if (isPouring)
            {
                targetPosition = pourAnchor.localPosition;
                targetRotation = pourAnchor.localRotation;
            }
            else
            {
                targetPosition = potOriLocalPosition;
                targetRotation = potOriLocalRotation;
                SetPourOilAnim(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isInPlace && isPouring)
            {
                targetRotation = pourAnchor.localRotation * pourRot;
            }
            SetPourOilAnim(true);
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            if (isInPlace && isPouring)
            {
                targetRotation = pourAnchor.localRotation;
            }
            SetPourOilAnim(false);
        }

        if (Vector3.Magnitude(potTrans.localPosition - targetPosition) > 0.1f ||
            Quaternion.Angle(potTrans.localRotation, targetRotation) > 0.1f)
        {
            Debug.Log("osa");
            potTrans.localPosition = Vector3.Lerp(potTrans.localPosition, targetPosition, Time.deltaTime * moveSpeed);
            potTrans.localRotation = Quaternion.Lerp(potTrans.localRotation, targetRotation, Time.deltaTime * moveSpeed);
        }
        else
        { 
            if (!isInPlace)
                isInPlace = true;
        }
    }

    void SetPourOilAnim(bool isStart)
    {
        if (isInPlace && isPouring)
        {
            pourOilAnimator.gameObject.SetActive(isStart);
            pourOilAnimator.SetBool("isPouring", isStart);
        }
        else
        {
            pourOilAnimator.gameObject.SetActive(false);
            pourOilAnimator.SetBool("isPouring", false);
        }
    }

    void ResetOilPotState()
    {
        targetPosition = potOriLocalPosition;
        targetRotation = potOriLocalRotation;
        potTrans.localPosition = potOriLocalPosition;
        potTrans.localRotation = potOriLocalRotation;
        isPouring = false;
        isInPlace = true;
    }
}
