using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CTable : Table
{
    [SerializeField]
    GameObject cCamera;
    ChoppingBlock choppingBlock;
    GameObject[] sliced = new  GameObject[1000] ;
    Transform choppingBlockTR;
    Transform bowl;

    protected override void Awake()
    {
        base.Awake();
        choppingBlock = transform.Find("UtensilSet").gameObject.GetComponentInChildren<ChoppingBlock>();
        choppingBlockTR = transform.Find("UtensilSet").Find("Choppingblock");
        bowl = transform.Find("UtensilSet").Find("Bowl");
    }

    protected override void Start()
    {
        thisRowMaxPlaceNum = 2;
        thisMaxPlaceNum = 4;
        thisColumnFoodSetSpace = -3.37f;
        thisRowFoodSetSpace = 1.89f;
        wares = new List<Container>(thisMaxPlaceNum);

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!isEnter)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("find begin");
            GetSliced();
            Debug.Log("find done");
        }

        if (Input.GetKeyDown(KeyCode.P))
            if (currentChosenWare != null && currentChosenWare.Contents.Count > 0)
                currentChosenWare.TakeOneTo(currentChosenWare.Contents[Random.Range(0, currentChosenWare.Contents.Count)], choppingBlock);

        if (Input.GetKeyDown(KeyCode.O))
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

        SetUtensilState(true);
        PutWareOnTablePreelectionPos();
        GameManager.Instance.uiManager.PushPanel(new CTableHintPanel());
    }

    protected override void OnQuitTable()
    {
        base.OnQuitTable();

        SetUtensilState(false);
        GivePlayerSelectedWare();
        GameManager.Instance.uiManager.PopPanel();
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

    private void GetSliced()
    {
        int childnum = 0;
        foreach(Transform slicedmaterials in choppingBlockTR)
        {
          //  Debug.Log(slicedmaterials.gameObject.name);
            sliced[childnum] = slicedmaterials.gameObject;
            Debug.Log(sliced[childnum].name);
        }
        for(int k = 1; k <= childnum; k++)
        {
           //从1开始拿
        }
    }

    [ContextMenu("ResetWaresPos")]
    public void ResetWaresPos()
    {
        Awake();
        ResetWaresPos(thisRowMaxPlaceNum, thisColumnFoodSetSpace, thisRowFoodSetSpace);
    }
}
