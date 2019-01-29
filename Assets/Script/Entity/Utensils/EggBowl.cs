using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBowl : Utensil
{
    Rigidbody chopsticksRb;
    Transform eggBowlAnchorTrans;
    List<Transform> eggBallsTrans;
    [SerializeField]
    Transform eggPlaneTrans;
    Transform bowlTrans;
    [SerializeField]
    Transform containedEggTrans;
    Vector3 targetPosition;
    Quaternion targetRotation;
    Vector3 bowlOriLocalPosition;
    Vector3 oriEggPlaneScale;
    Transform rollChopsticksAnchorTrans;
    Transform preparingChopsticksAnchorTrans;
    Quaternion bowlOriLocalRotation;
    Animator crackAnimator;
    Material yolkMat;
    Material albumenMat;
    Material oriAlbumenMat;

    bool isBeatingEgg;
    bool isInPlace;
    float moveSpeed;
    int currentEggBallCount;
    int maxEggBallCount;
    float eggPlaneRaseDistance;
    float eggPlaneScaleDistance;
    float chopsticksRollSpeed;
    float stirTimer;

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
        rollChopsticksAnchorTrans = this.transform.Find("Bowl/RollChopsticksAnchor");
        preparingChopsticksAnchorTrans = this.transform.Find("Bowl/PreparingChopsticksAnchor");
        eggBallsTrans = GetTransListInChildren(bowlTrans,"ContainedEgg/EggBall");
        yolkMat = eggBallsTrans[0].GetComponent<Renderer>().material;
        albumenMat = eggPlaneTrans.GetComponent<Renderer>().material;
        oriAlbumenMat = Resources.Load<Material>("Materials/OriAlbumen");
        oriEggPlaneScale = eggPlaneTrans.localScale;
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
        eggPlaneRaseDistance = 0.02f;
        eggPlaneScaleDistance = 0.04f; 
        chopsticksRollSpeed = 20f;
        stirTimer = 0f;
    }

    public override void OnBeginCtrl()
    {
        base.OnBeginCtrl();
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        MoveToolToTargetPos(
            bowlTrans.transform, targetPosition, 
            targetRotation, moveSpeed, ref isInPlace);

        SetEggBowlTargetPos();
        PlusEggEffect();
        StirEggEffect();

        if (isBeatingEgg)
        {
            if (chopsticksRb.useGravity == true)
            {
                SetRigidbody(false);
                PreparingChopsticksPos();
            }
        }
        else
        {
            if (chopsticksRb.useGravity == false&&isInPlace)
                SetRigidbody(true);
        }
    }

    public override void OnStopCtrl()
    {
        base.OnStopCtrl();

        SetRigidbody(true);
        ResetEggBowlState();
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
        isBeatingEgg = false;
        bowlTrans.transform.localPosition = bowlOriLocalPosition;
        bowlTrans.transform.localRotation = bowlOriLocalRotation;
        targetPosition = bowlOriLocalPosition;
        targetRotation = bowlOriLocalRotation;
    }

    void PlusEggEffect()
    {
        if (this.ingredients.Count > currentEggBallCount
            && currentEggBallCount <= maxEggBallCount
            && crackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        {
            currentEggBallCount++;
            stirTimer = 0f; 
            if (currentEggBallCount == 1)
            {
                eggPlaneTrans.gameObject.SetActive(true);
            }
            else if (currentEggBallCount > 1)
            {
                albumenMat.color = Color.Lerp(albumenMat.color, oriAlbumenMat.color,1f/ currentEggBallCount);
            }
            eggBallsTrans[currentEggBallCount - 1].gameObject.SetActive(true);
            containedEggTrans.localPosition += new Vector3(0, eggPlaneRaseDistance, 0);
            eggPlaneTrans.localScale += new Vector3(eggPlaneScaleDistance, 0, eggPlaneScaleDistance);
        }
    }

    void StirEggEffect()
    {
        if (Input.GetKey(KeyCode.Space)&& isBeatingEgg)
        {
            Debug.Log("rolling");

            StopCoroutine("EggRotatoryInertia");
            chopsticksRb.transform.RotateAround(rollChopsticksAnchorTrans.position, Vector3.up, chopsticksRollSpeed);
            chopsticksRb.transform.localRotation = preparingChopsticksAnchorTrans.localRotation;
            containedEggTrans.Rotate(Vector3.up, chopsticksRollSpeed*0.5f);

            if (currentEggBallCount > 0)
            {
                stirTimer += Time.deltaTime;
                if (stirTimer >= 3f)
                {
                    albumenMat.color = Color.Lerp(albumenMat.color, yolkMat.color, Time.deltaTime);

                    for (int i = 0; i< currentEggBallCount; i++)
                    {
                        eggBallsTrans[i].gameObject.SetActive(false);
                        ingredients[i].GetComponent<Egg>().EggState = EggState.Stirred;
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isBeatingEgg)
        {
            float eggRollSpeed = chopsticksRollSpeed*0.5f;
            StartCoroutine("EggRotatoryInertia", eggRollSpeed);
        }
    }

    IEnumerator EggRotatoryInertia(float rollSpeed)
    {    
        yield return null;
        while (true)
        {
            rollSpeed -= rollSpeed*0.2f;
            containedEggTrans.Rotate(Vector3.up, rollSpeed);

            if (rollSpeed <= 0)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void PreparingChopsticksPos()
    {
        if (chopsticksRb.transform.localRotation != preparingChopsticksAnchorTrans.localRotation ||
chopsticksRb.transform.localPosition != preparingChopsticksAnchorTrans.localPosition)
        {
            chopsticksRb.transform.localPosition = preparingChopsticksAnchorTrans.localPosition;
            chopsticksRb.transform.localRotation = preparingChopsticksAnchorTrans.localRotation;
        }
    }

    List<Transform> GetTransListInChildren(Transform parent, string childrenPath)
    {
        List<Transform> transforms = new List<Transform>();
        Transform transform = parent.Find(childrenPath).GetComponent<Transform>();
        foreach (Transform temp in transform)
        {
            transforms.Add(temp);
        }
        return transforms;
    }

    public void ResetEggBowl()
    {
        foreach (Transform temp in eggBallsTrans)
        {
            temp.gameObject.SetActive(false); 
        }
        eggPlaneTrans.localScale = oriEggPlaneScale;
        //albumenMat.color = oriAlbumenMat.color;
        eggPlaneTrans.gameObject.SetActive(false);
        stirTimer = 0f;
        currentEggBallCount = 0;
        this.Contents.Clear();
    }
}
