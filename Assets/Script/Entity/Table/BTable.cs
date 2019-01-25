using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BTable : Table
{

    [SerializeField]
    GameObject bCamera;

    private Pot pot;

    protected override void Awake()
    {
        base.Awake();

        thisRowMaxPlaceNum = 2;
        thisMaxPlaceNum = 4;
        thisColumnFoodSetSpace = 4.37f;
        thisRowFoodSetSpace = -1.4f;
    }

    protected override void Start()
    {
        wareSet = new List<Container>(thisMaxPlaceNum);
        GetUtensil();

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
            if (currentChosenWare != null)
            {
                currentChosenWare.TakeOutAllTo(pot);
            }
    }

    protected override void GetCamera()
    {
        cameraGO = bCamera;
    }

    private void GetUtensil()
    {
        if (transform.Find("UtensilSet") != null)
        {
            pot = transform.Find("UtensilSet").gameObject.GetComponentInChildren<Pot>();
        }
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

        SetUtensilState(true);
        PutWareOnTablePreelectionPos();
    }

    protected override void OnQuitTable()
    {
        base.OnQuitTable();
        SetUtensilState(false);
        GivePlayerSelectedWare();
    }

    private void SetUtensilState(bool isBeginCtrl)
    {
        if (pot != null)
        {
            if (isBeginCtrl)
                pot.BeginCtrl();
            else
                pot.StopCtrl();
        }
    }

    [ContextMenu("ResetWaresPos")]
    public void ResetWaresPos()
    {
        Awake();

        ResetWaresPos(thisRowMaxPlaceNum, thisColumnFoodSetSpace, thisRowFoodSetSpace);
    }
}
