using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBowl : Utensil
{
    Rigidbody chopsticksRb;
    Transform eggBowlAnchorTrans;
    [SerializeField]
    Transform[] eggBallsTrans;
    [SerializeField]
    Transform eggPlaneTrans;
    Transform bowlTrans;
    [SerializeField]
    Transform containedEggTrans;
    Vector3 targetPosition;
    Quaternion targetRotation;
    Vector3 bowlOriLocalPosition;
    Quaternion bowlOriLocalRotation;
    Animator crackAnimator;

    bool isBeatingEgg;
    bool isInPlace;
    float moveSpeed;
    int currentEggBallCount;
    int maxEggBallCount;
    float eggPlaneRaseDistance;
    float eggPlaneScaleDistance;

    public bool IsBeatingEgg
    {
        get
        {
            return isBeatingEgg;
        }

        set
        {
            isBeatingEgg = value;
        }
    }

    public bool IsInPlace
    {
        get
        {
            return isInPlace;
        }

        set
        {
            isInPlace = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        bowlTrans = this.transform.Find("Bowl");
        eggBowlAnchorTrans = this.transform.Find("EggBowlAnchor");
        chopsticksRb = bowlTrans.transform.Find("Chopsticks").GetComponent<Rigidbody>();
        crackAnimator = this.transform.Find("Bowl/Crack").gameObject.GetComponent<Animator>();
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
        currentEggBallCount = 0;
        maxEggBallCount = 7;
        eggPlaneRaseDistance = 0.1f;
        eggPlaneScaleDistance = 0.1f;
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
        PlusEggEffect();
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
                targetPosition = eggBowlAnchorTrans.localPosition;
                targetRotation = eggBowlAnchorTrans.localRotation;
            }
            else
            {
                targetPosition = bowlOriLocalPosition;
                targetRotation = bowlOriLocalRotation;
            }
        }
    }

    void ResetEggBowlState()
    {

    }

    void PlusEggEffect()
    {
        if (this.ingredients.Count > currentEggBallCount
            && currentEggBallCount <= maxEggBallCount
            && crackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        {
            if (currentEggBallCount == 0)
            {
                eggPlaneTrans.gameObject.SetActive(true);
            }
            currentEggBallCount++;
            eggBallsTrans[currentEggBallCount - 1].gameObject.SetActive(true);
            containedEggTrans.localPosition += new Vector3(0, eggPlaneRaseDistance, 0);
            eggPlaneTrans.localScale += new Vector3(eggPlaneScaleDistance, 0, eggPlaneScaleDistance);
        }
    }
}
