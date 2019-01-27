using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PTable : Table
{
    [SerializeField]
    GameObject pCamera;
    EggBowl eggBowl;
    Animator crackAnimator;


    protected override void Awake()
    {
        base.Awake();
        eggBowl = transform.Find("UtensilSet").gameObject.GetComponentInChildren<EggBowl>();
        crackAnimator = eggBowl.transform.GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        wareSet = new List<Container>(thisMaxPlaceNum);
        thisRowMaxPlaceNum = 1;
        thisMaxPlaceNum = 2;
        thisColumnFoodSetSpace = -4.28f;
        thisRowFoodSetSpace = 0f;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        PutEggToEggBowl();
    }

    private void PutEggToEggBowl()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentChosenWare != null 
                && currentChosenWare.Contents.Count > 0 
                && crackAnimator.GetBool("isCrack")==false 
                && currentChosenWare.Contents[0].FoodName == FoodName.Egg)
            {
                crackAnimator.SetBool("isCrack", true);
                currentChosenWare.TakeOneTo(currentChosenWare.Contents[Random.Range(0, currentChosenWare.Contents.Count)], eggBowl);
            }
        }
    }

    private void SetCrackAnim()
    {

    }

    protected override void GetCamera()
    {
        cameraGO = pCamera;
    }

    protected override void SelectFoodSet()
    {
        base.SelectFoodSet();
        if (currentChosenWare != null)
        {
            if (Input.GetMouseButtonDown(1) && selectFoodSetCoroutine == null)
            {
                PutFoodSetBack();
            }
        }
    }

    protected override void SelectMethod()
    {
        base.SelectMethod();
        PutFoodSetBack();
        wareSet[wareSetIndex].transform.position = preelectionFoodSetTrans.position;
        currentChosenWare = wareSet[wareSetIndex].gameObject.GetComponent<Ware>();
        wareSet[wareSetIndex] = null;
    }

    protected override void OnEnterTable()
    {
        base.OnEnterTable();
        PutWareOnTablePreelectionPos();
    }

    protected override void OnQuitTable()
    {
        base.OnQuitTable();
        GivePlayerSelectedWare();
    }

    [ContextMenu("ResetWaresPos")]
    public void ResetWaresPos()
    {
        Awake();

        ResetWaresPos(thisRowMaxPlaceNum, thisColumnFoodSetSpace, thisRowFoodSetSpace);
    }
}
