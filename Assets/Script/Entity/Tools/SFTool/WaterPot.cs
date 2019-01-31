using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPot : Tool {

    Rigidbody potRb;
    Transform potTrans;
    Vector3 potOriLocalPosition;
    Quaternion potOriLocalRotation;
    Vector3 targetPosition;
    Quaternion targetRotation;

    Quaternion pourRot;
    Transform pourAnchorTrans;
    Animator pourWaterAnimator;
    GameObject waterplane;
    GameObject waterpouringanimation;

    bool isPouring;
    bool iswaterplacerising;
    bool isInPlace;

    float moveSpeed;

    protected override void Awake()
    {
        waterplane = this.transform.Find("waterPlane").gameObject;
        potTrans = this.transform.Find("Pot");
        pourAnchorTrans = this.transform.Find("PourAnchor");
        potRb = potTrans.GetComponent<Rigidbody>();
        pourWaterAnimator = potTrans.Find("PourWater").GetComponent<Animator>();
        waterpouringanimation = potTrans.Find("PourWater").gameObject;
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
        pourWaterAnimator.gameObject.SetActive(false);
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
        MoveToolToTargetPos(potTrans, targetPosition, targetRotation, moveSpeed, ref isInPlace);
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
        if (Input.GetKeyDown(KeyCode.Alpha3))
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
                pourWaterAnimator.gameObject.SetActive(false);
                pourWaterAnimator.SetBool("isPouring", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isPouring = false;
            isInPlace = false;
            targetPosition = potOriLocalPosition;
            targetRotation = potOriLocalRotation;
            pourWaterAnimator.gameObject.SetActive(false);
            pourWaterAnimator.SetBool("isPouring", false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isPouring = false;
            isInPlace = false;
            targetPosition = potOriLocalPosition;
            targetRotation = potOriLocalRotation;
            pourWaterAnimator.gameObject.SetActive(false);
            pourWaterAnimator.SetBool("isPouring", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isInPlace && isPouring)
            {
                //oilplane.transform.position = new Vector3(oilplane.transform.position.x, oilplane.transform.position.y+(Time.deltaTime), oilplane.transform.position.z);
                if (waterplane.transform.localScale.x < 0.9f)
                {

                    iswaterplacerising = true;
                }
                else
                {
                    iswaterplacerising = false;
                }

                targetRotation = pourAnchorTrans.localRotation * pourRot;
                pourWaterAnimator.gameObject.SetActive(true);
                pourWaterAnimator.SetBool("isPouring", true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isInPlace && isPouring)
            {
                waterplane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                iswaterplacerising = false;
                targetRotation = pourAnchorTrans.localRotation;
                pourWaterAnimator.gameObject.SetActive(false);
                pourWaterAnimator.SetBool("isPouring", false);
            }
        }
    }

    void ResetOilPotState()
    {
        pourWaterAnimator.gameObject.SetActive(false);
        pourWaterAnimator.SetBool("isPouring", false);
        targetPosition = potOriLocalPosition;
        targetRotation = potOriLocalRotation;
        potTrans.localPosition = potOriLocalPosition;
        potTrans.localRotation = potOriLocalRotation;
        isPouring = false;
        isInPlace = true;
    }

    private void FixedUpdate()
    {
        if (iswaterplacerising && waterplane.transform.localScale.x < 0.9f)
        {
            waterplane.transform.localScale = new Vector3(waterplane.transform.localScale.x + 0.01f * Time.deltaTime, waterplane.transform.localScale.y, waterplane.transform.localScale.z + 0.01f * Time.deltaTime);
            waterplane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.01f, 0);
        }
        else
        {
            waterplane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }
}
