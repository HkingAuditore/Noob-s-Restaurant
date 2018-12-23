using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SMTable : Table
{
    [SerializeField]
    private GameObject smCamera;
    [SerializeField]
    private GameObject tool;
    private Transform preelectionFoodSetTrans;
    private GameObject currentChosenFoodSetGO;//存储选择完成后被选中的 foodSet
    Vector3 PrePos;

    protected override void Awake()
    {
        base.Awake();
        preelectionFoodSetTrans = this.transform.Find("PreelectionFoodSetMark");
    }

    protected override void Start()
    {
        base.Start();
        thisCameraGO = smCamera;
        toolGo = tool;
    }

    protected override void SelectMethod(GameObject cBowl)
    {
        if (PrePos != null&&currentChosenFoodSetGO!=null)
        {
            currentChosenFoodSetGO.transform.position = PrePos;
        }
        PrePos = foodSetScripts[foodSetsIndex].transform.position;
        foodSetScripts[foodSetsIndex].transform.position = preelectionFoodSetTrans.position;
        currentChosenFoodSetGO = foodSetScripts[foodSetsIndex].gameObject;
    }

    protected override void CancelMethod(GameObject cBowl)
    {
    }

    protected override void OnEnterTable()
    {
        
    }

    protected override void OnQuitTable()
    {
        if (currentChosenFoodSetGO != null)
        {
            playerCtrlScript.SetPlayerHeldFoodSet(currentChosenFoodSetGO);
            foodSetScripts.Remove(currentChosenFoodSetGO.GetComponent<FoodSet>());
        }
    }
}
