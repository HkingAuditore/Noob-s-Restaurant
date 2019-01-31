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
    GameObject oilplane;
    GameObject oilpouringanimation;

    bool isPouring;
    bool isoilplacerising;
    bool isInPlace;

    float moveSpeed;

    protected override void Awake()
    {
        oilplane = this.transform.Find("OilPlane").gameObject;
        potTrans = this.transform.Find("Pot");
        pourAnchorTrans = this.transform.Find("PourAnchor");
        potRb = potTrans.GetComponent<Rigidbody>();
        pourOilAnimator = potTrans.Find("PourOil").GetComponent<Animator>();
        oilpouringanimation = potTrans.Find("PourOil").gameObject;
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

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isPouring = false;
            isInPlace = false;
            targetPosition = potOriLocalPosition;
            targetRotation = potOriLocalRotation;
            pourOilAnimator.gameObject.SetActive(false);
            pourOilAnimator.SetBool("isPouring", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isInPlace && isPouring)
            {
                //oilplane.transform.position = new Vector3(oilplane.transform.position.x, oilplane.transform.position.y+(Time.deltaTime), oilplane.transform.position.z);
                if (oilplane.transform.localScale.x < 0.9f)
                {
                    
                    isoilplacerising = true;
                }
                else
                {
                    isoilplacerising = false;
                }
                
                targetRotation = pourAnchorTrans.localRotation * pourRot;
                pourOilAnimator.gameObject.SetActive(true);
                pourOilAnimator.SetBool("isPouring", true);
            }
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            if (isInPlace && isPouring)
            {
                oilplane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                isoilplacerising = false;
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

    private void FixedUpdate()
    {
        if (isoilplacerising && oilplane.transform.localScale.x < 0.9f)
        {
            oilplane.transform.localScale = new Vector3(oilplane.transform.localScale.x+0.01f*Time.deltaTime, oilplane.transform.localScale.y, oilplane.transform.localScale.z + 0.01f * Time.deltaTime);
            oilplane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.01f, 0);
        }
        else
        {
            oilplane.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }
}
