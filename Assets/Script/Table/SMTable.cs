﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SMTable : Table
{
    [SerializeField]
    private GameObject smCamera;
    [SerializeField]
    private GameObject toolSet;
    private Transform foodSetTrans;
    private Transform preelectionFoodSetTrans;
    private Transform _11MarkTrans;
    [SerializeField]
    private int thisRowMaxPlaceNum = 5;//控制此桌一排的最大放碗数
    [SerializeField]
    private int thisMaxPlaceNum = 10;//控制此桌最大容量
    [SerializeField]
    private float thisColumnFoodSetSpace = 0.63f;//用于设置桌子上foodset间的列间隔 
    [SerializeField]
    private float thisRowFoodSetSpace = 0.57f;//用于设置桌子上foodset间的行间隔

    protected override void Awake()
    {
        base.Awake();
        preelectionFoodSetTrans = this.transform.Find("PreelectionFoodSetMark");
        foodSetTrans = this.transform.Find("FoodSet");
        _11MarkTrans = this.transform.Find("11Mark");
    }

    protected override void Start()
    {
        base.Start();
        thisCameraGO = smCamera;
        toolGo = toolSet;
    }

    protected override void SelectFoodSet()
    {
        base.SelectFoodSet();
        if (currentChosenFoodSetGO != null)
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
        foodSetScripts[foodSetScriptsIndex].transform.position = preelectionFoodSetTrans.position;
        currentChosenFoodSetGO = foodSetScripts[foodSetScriptsIndex].gameObject;
        foodSetScripts[foodSetScriptsIndex] = null;
    }

    protected override void OnEnterTable()
    {
        PutFoodSetOnTablePreelectionPos();
    }

    protected override void OnQuitTable()
    {
        base.OnQuitTable();
        GivePlayerFoodSet();
    }

    private void PutFoodSetBack()
    {
        if (currentChosenFoodSetGO != null)
        {
            int i;
            for (i = 0; i < thisMaxPlaceNum; i++)
            {
                if (foodSetScripts[i] == null)
                {
                    Debug.Log(i + "号位孔雀");
                    Debug.Log((i * thisRowFoodSetSpace));

                    if (i >= thisRowMaxPlaceNum)
                    {
                        currentChosenFoodSetGO.transform.localPosition =
                            new Vector3(_11MarkTrans.localPosition.x -  thisColumnFoodSetSpace, _11MarkTrans.localPosition.y, _11MarkTrans.localPosition.z - ((i - thisRowMaxPlaceNum) * thisRowFoodSetSpace));
                    }
                    else
                    {
                        currentChosenFoodSetGO.transform.localPosition =
                            new Vector3(_11MarkTrans.localPosition.x, _11MarkTrans.localPosition.y, _11MarkTrans.localPosition.z - (i * thisRowFoodSetSpace));
                    }
                    foodSetScripts[i] = currentChosenFoodSetGO.GetComponent<FoodSet>();
                    currentChosenFoodSetGO = null;
                    break;
                }
            }
            if (i > thisRowMaxPlaceNum)
            {
                Debug.Log("此桌已经没有空位了");
                return;
            }
        }
    }

    private void PutFoodSetOnTablePreelectionPos()
    {
        if (playerCtrlScript.isHoldFoodSet)
        {
            GameObject holdFoodSet = playerCtrlScript.transform.Find("Model/metarig.001").transform.GetComponentInChildren<FoodSet>().gameObject;
            //holdFoodSet.SetActive(false);
            holdFoodSet.transform.position = preelectionFoodSetTrans.position;
            //holdFoodSet.SetActive(true);
            holdFoodSet.transform.SetParent(foodSetTrans);
            currentChosenFoodSetGO = holdFoodSet;
            playerCtrlScript.isHoldFoodSet = false;

        }
    }

    private void GivePlayerFoodSet()
    {
        if (playerCtrlScript.isHoldFoodSet)
        {
            Debug.Log("已经持有" + playerCtrlScript.heldFoodSet.name);
            return;
        }
        if (currentChosenFoodSetGO != null)
        {
            currentChosenFoodSetGO.transform.position = playerCtrlScript.holdFoodMarkTrans.position;
            currentChosenFoodSetGO.transform.SetParent(playerCtrlScript.transform.Find("Model/metarig.001").transform);
            playerCtrlScript.heldFoodSet = currentChosenFoodSetGO;
            playerCtrlScript.isHoldFoodSet = true;
            currentChosenFoodSetGO = null;
        }
    }
}
