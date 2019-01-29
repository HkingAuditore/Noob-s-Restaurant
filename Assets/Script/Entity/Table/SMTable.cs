using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SMTable : Table
{
    [SerializeField]
    private GameObject smCamera;

    protected override void Awake()
    {
        base.Awake();

        thisRowMaxPlaceNum = 5;
        thisMaxPlaceNum = 10;
        thisColumnFoodSetSpace = 1.25f;
        thisRowFoodSetSpace = 1.2f;
    }

    protected override void Start()
    {
        wares = new List<Container>(thisMaxPlaceNum);

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!isEnter)
            return;
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
        wares[wareSetIndex].transform.position = preelectionFoodSetTrans.position;
        currentChosenWare = wares[wareSetIndex].gameObject.GetComponent<Ware>();
        wares[wareSetIndex] = null;
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
