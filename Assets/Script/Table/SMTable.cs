using System.Collections;
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
    private float FoodSetSpace;//用于设置桌子上foodset间的间隔，默认为0.5

    protected override void Awake()
    {
        base.Awake();
        preelectionFoodSetTrans = this.transform.Find("PreelectionFoodSetMark");
        foodSetTrans = this.transform.Find("FoodSet");
        _11MarkTrans = this.transform.Find("11Mark");
        maxPlaceNum = 5;
    }

    protected override void Start()
    {
        base.Start();
        FoodSetSpace = 0.5f;
        thisCameraGO = smCamera;
        toolGo = toolSet;
    }

    protected override void SelectFoodSet()
    {
        base.SelectFoodSet();
        Debug.Log("aaaa");
        if (currentChosenFoodSetGO != null)
        {
            if (Input.GetMouseButtonDown(1) && selectFoodSetCoroutine == null)
            {
                Debug.Log("ppp");
                PutFoodSetBack();
            }
        }
    }

    protected override void SelectMethod(GameObject cBowl)
    {
        base.SelectMethod(cBowl);
        PutFoodSetBack();
        foodSetScripts[foodSetScriptsIndex].transform.position = preelectionFoodSetTrans.position;
        currentChosenFoodSetGO = foodSetScripts[foodSetScriptsIndex].gameObject;
        foodSetScripts[foodSetScriptsIndex] = null;
    }

    protected override void CancelMethod(GameObject cBowl)
    {
        base.CancelMethod(cBowl);
    }

    protected override void OnEnterTable()
    {
        PutFoodSetOnTablePreelectionPos();
    }

    protected override void OnQuitTable()
    {
        GivePlayerFoodSet();
    }

    private void PutFoodSetBack()
    {
        if (currentChosenFoodSetGO != null)
        {
            int i;
            for (i = 0; i < maxPlaceNum; i++)
            {
                if (foodSetScripts[i] == null)
                {
                    Debug.Log(i + "号位孔雀");
                    Debug.Log((i * FoodSetSpace));
                    currentChosenFoodSetGO.transform.localPosition =
                        new Vector3(_11MarkTrans.localPosition.x, _11MarkTrans.localPosition.y, _11MarkTrans.localPosition.z - (i * FoodSetSpace));
                    foodSetScripts[i] = currentChosenFoodSetGO.GetComponent<FoodSet>();
                    currentChosenFoodSetGO = null;
                    break;
                }
            }
            if (i >= maxPlaceNum)
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
            holdFoodSet.transform.position = preelectionFoodSetTrans.position;
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
