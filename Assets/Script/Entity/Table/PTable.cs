using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PTable : Table
{

    [SerializeField]
    GameObject pCamera;

    protected override void Awake()
    {
        base.Awake();

        thisRowMaxPlaceNum = 1;
        thisMaxPlaceNum = 2;
        thisColumnFoodSetSpace = -4.28f;
        thisRowFoodSetSpace = 0f;
    }

    protected override void Start()
    {
        wareSet = new List<Container>(thisMaxPlaceNum);

        base.Start();
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
