using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CTable : Table, IContainable<Container>
{

    [SerializeField]
    GameObject cCamera;

    private ChoppingBlock choppingBlock;

    protected override void Awake()
    {
        base.Awake();

        thisRowMaxPlaceNum = 2;
        thisMaxPlaceNum = 4;
        thisColumnFoodSetSpace = -3.37f;
        thisRowFoodSetSpace = 1.89f;
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
            if (currentChosenWare != null && currentChosenWare.Contents.Count > 0)
                currentChosenWare.TakeOneTo(currentChosenWare.Contents[Random.Range(0, currentChosenWare.Contents.Count)], choppingBlock);

        if (Input.GetKey(KeyCode.O))
            if (currentChosenWare != null && choppingBlock.Contents.Count > 0)
            {
                //OPT
                choppingBlock.Contents.Clear();
                choppingBlock.Contents.AddRange(choppingBlock.GetComponentsInChildren<Ingredient>());

                choppingBlock.TakeOneTo(choppingBlock.Contents[Random.Range(0, choppingBlock.Contents.Count)], currentChosenWare);
            }
    }

    protected override void GetCamera()
    {
        cameraGO = cCamera;
    }

    private void GetUtensil()
    {
        if (transform.Find("UtensilSet") != null)
        {
            choppingBlock = transform.Find("UtensilSet").gameObject.GetComponentInChildren<ChoppingBlock>();
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
        if (choppingBlock != null)
        {
            if (isBeginCtrl)
                choppingBlock.BeginCtrl();
            else
                choppingBlock.StopCtrl();
        }
    }

    [ContextMenu("ResetWaresPos")]
    public void ResetWaresPos()
    {
        Awake();

        ResetWaresPos(thisRowMaxPlaceNum, thisColumnFoodSetSpace, thisRowFoodSetSpace);
    }
}
