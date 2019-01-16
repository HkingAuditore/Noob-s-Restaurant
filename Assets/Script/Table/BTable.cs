using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BTable : Table {

    [SerializeField]
    GameObject bCamera;

    protected override void Awake()
    {
        base.Awake();

        thisRowMaxPlaceNum = 2;
        thisMaxPlaceNum = 4;
        thisColumnFoodSetSpace = 5.85f;
        thisRowFoodSetSpace = -2.02f;

        wareSet = new List<Container>(thisMaxPlaceNum);
    }

    protected override void GetCamera()
    {
        cameraGO = bCamera;
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
}
