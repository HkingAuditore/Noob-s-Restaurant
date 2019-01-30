using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltPot : Tool {

    Transform sprinkleAnchorTrans;
    Transform potTrans;
    Vector3 targetPosition;
    Quaternion targetRotation;
    Vector3 potOriLocalPosition;
    Quaternion potOriLocalRotation;
    Quaternion sprinkleRot;
    GameObject salt_particle;


    bool isSprinkling;
    bool isInPlace;
    float moveSpeed;

    protected override void Awake()
    {
        base.Awake();

        sprinkleAnchorTrans = this.transform.Find("SprinkleAnchor");
        salt_particle = sprinkleAnchorTrans.Find("salt_particle").gameObject;
        potTrans = this.transform.Find("Pot");
    }

    protected override void Start()
    {
        base.Start();
        isSprinkling = false;
        isInPlace = true;
        moveSpeed = 2.5f;
        potOriLocalPosition = potTrans.localPosition;
        potOriLocalRotation = potTrans.localRotation;
        sprinkleRot = Quaternion.AngleAxis(-135f, Vector3.right);
    }

    public override void OnBeginCtrl()
    {
        base.OnBeginCtrl();
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        SetSaltPotTargetPos();
        MoveToolToTargetPos(potTrans, targetPosition, targetRotation, moveSpeed, ref isInPlace);
    }

    public override void OnStopCtrl()
    {
        base.OnStopCtrl();
    }

    void SetSaltPotTargetPos()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isSprinkling = !isSprinkling;
            isInPlace = false;

            if (isSprinkling)
            {
                targetPosition = sprinkleAnchorTrans.localPosition;
                targetRotation = sprinkleAnchorTrans.localRotation;
            }
            else
            {
                targetPosition = potOriLocalPosition;
                targetRotation = potOriLocalRotation;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isSprinkling = false;
            isInPlace = false;
            targetPosition = potOriLocalPosition;
            targetRotation = potOriLocalRotation;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            if (isInPlace && isSprinkling)
            {
                salt_particle.SetActive(true);
                targetRotation = sprinkleAnchorTrans.localRotation * sprinkleRot;
                   
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isInPlace && isSprinkling)
            {
                salt_particle.SetActive(false);
                targetRotation = sprinkleAnchorTrans.localRotation;
                
            }
        }
    }

    void ResetSaltPot()
    {
        potTrans.localPosition = potOriLocalPosition;
    }
}
