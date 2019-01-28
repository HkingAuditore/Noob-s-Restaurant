using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBowl : Utensil
{
    Rigidbody chopsticksRb;
    Transform EggBowlAnchorTrans;
    Transform bowlTrans;
    Vector3 targetPosition;
    Quaternion targetRotation;
    Vector3 bowlOriLocalPosition;
    Quaternion bowlOriLocalRotation;

    bool isBeatingEgg;
    bool isInPlace;
    float moveSpeed;

    protected override void Awake()
    {
        base.Awake();
        bowlTrans = this.transform.Find("Bowl");
        EggBowlAnchorTrans = this.transform.Find("EggBowlAnchor");
        chopsticksRb = bowlTrans.transform.Find("Chopsticks").GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();

        isBeatingEgg = false;
        isInPlace = true;
        moveSpeed = 2.5f;
        bowlOriLocalPosition = bowlTrans.transform.localPosition;
        bowlOriLocalRotation = bowlTrans.transform.localRotation;
        targetPosition = bowlOriLocalPosition;
        targetRotation = bowlOriLocalRotation;
    }

    public override void OnBeginCtrl()
    {
        base.OnBeginCtrl();
        SetRigidbody(false);
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        SetEggBowlTargetPos();
        MoveToolToTargetPos(bowlTrans.transform, targetPosition, targetRotation, moveSpeed, ref isInPlace);
    }

    public override void OnStopCtrl()
    {
        base.OnStopCtrl();
    }


    private void SetRigidbody(bool isTrue)
    {
        if (!isTrue)
        {
            chopsticksRb.useGravity = false;
            chopsticksRb.isKinematic = true;
        }
        else
        {
            chopsticksRb.useGravity = true;
            chopsticksRb.isKinematic = false;
        }
    }

    private void SetEggBowlTargetPos()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isBeatingEgg = !isBeatingEgg;
            isInPlace = false;

            if (isBeatingEgg)
            {
                targetPosition = EggBowlAnchorTrans.localPosition;
                targetRotation = EggBowlAnchorTrans.localRotation;
            }
            else
            {
                targetPosition = bowlOriLocalPosition;
                targetRotation = bowlOriLocalRotation;
            }
        }
    }
}
