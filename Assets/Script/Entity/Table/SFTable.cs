using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SFTable : Table, IContainable<Container>
{

    [SerializeField]
    GameObject sfCamera;

    private Utensil utensil;

    protected override void Awake()
    {
        base.Awake();

        thisRowMaxPlaceNum = 2;
        thisMaxPlaceNum = 4;
        thisColumnFoodSetSpace = 5.85f;
        thisRowFoodSetSpace = -2.02f;
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
                currentChosenWare.TakeOutAllTo(utensil);
    }

    protected override void GetCamera()
    {
        cameraGO = sfCamera;
    }

    private void GetUtensil()
    {
        if (transform.Find("UtensilSet") != null)
        {
            utensil = transform.Find("UtensilSet").gameObject.GetComponentInChildren<Utensil>();
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

        SetUtensil(true);
        PutWareOnTablePreelectionPos();
    }

    protected override void OnQuitTable()
    {
        base.OnQuitTable();

        SetUtensil(false);
        GivePlayerSelectedWare();
    }

    private void SetUtensil(bool isBeginCtrl)
    {
        if (utensil != null)
        {
            if (isBeginCtrl)
                utensil.BeginCtrl();
            else
                utensil.StopCtrl();
        }
    }

    [ContextMenu("ResetWaresPos")]
    public void ResetWaresPos()
    {
        Awake();

        ResetWaresPos(thisRowMaxPlaceNum, thisColumnFoodSetSpace, thisRowFoodSetSpace);
    }
}
