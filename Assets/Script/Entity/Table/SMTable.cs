using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SMTable : Table, IContainable<Container>
{
    [SerializeField]
    private GameObject smCamera;

    protected override void Awake()
    {
        base.Awake();

        thisRowMaxPlaceNum = 5;
        thisMaxPlaceNum = 10;
        thisColumnFoodSetSpace = 0.63f;
        thisRowFoodSetSpace = 0.57f;
    }

    protected override void Start()
    {
        wareSet = new List<Container>(thisMaxPlaceNum);

        base.Start();
    }

    protected override void GetCamera()
    {
        cameraGO = smCamera;
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
