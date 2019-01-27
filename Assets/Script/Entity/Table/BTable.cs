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

        HeatTimer = new Timer(new float[] { 30, 60, 90 }, false);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
            if (currentChosenWare != null)
            {
                Debug.Log(currentChosenWare.Contents.Count);
                currentChosenWare.TakeOutAllTo(pot);
            }

        if (Input.GetKey(KeyCode.O))
            if (currentChosenWare != null && pot.Contents.Count > 0)
            {
                pot.TakeOneTo(pot.Contents[Random.Range(0, pot.Contents.Count)], currentChosenWare);
            }
    }

    private void OnGUI()
    {
        if (!isEnter)
            return;
        GUIStyle style = new GUIStyle
        {
            fontSize = 40,
            fontStyle = FontStyle.Bold
        };
        GUI.Label(new Rect(20, 20, 200, 100), "Heating : " + HeatTimer.CurTime.ToString("F2") + "s", style);
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
