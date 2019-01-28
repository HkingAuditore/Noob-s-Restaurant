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
        wares = new List<Container>(thisMaxPlaceNum);
        GetUtensil();

        base.Start();

        HeatTimer = new Timer(new float[] { 30, 60, 90 }, false);
        HeatTimer.onStop += OnHeatStop;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
            PutIngredientsFromCurChosenWareToPot();

        if (Input.GetKey(KeyCode.O))
            PutIngredientsFromPotToCurChosenWare();
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
        GUI.Label(new Rect(20, 20, 200, 100), "Heating : " + HeatTimer.SumTime.ToString("F2") + "s", style);
    }

    private void OnDestroy()
    {
        HeatTimer.onStop -= OnHeatStop;
    }

    private void PutIngredientsFromCurChosenWareToPot()
    {
        if (currentChosenWare != null)
        {
            currentChosenWare.TakeOutAllTo(pot);
        }
    }

    private void PutIngredientsFromPotToCurChosenWare()
    {
        if (currentChosenWare != null && pot.Contents.Count > 0)
        {
            Ingredient ingredient = pot.Contents[Random.Range(0, pot.Contents.Count)];
            pot.TakeOneTo(ingredient, currentChosenWare);
            Debug.Log(ingredient.HeatTime);
        }
    }

    private void OnHeatStop(float onceTime, float sumTime)
    {
        pot.Contents.ForEach((Ingredient) => Ingredient.UpdateHeatTime(onceTime));
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
        wares[wareSetIndex].transform.position = preelectionFoodSetTrans.position;
        currentChosenWare = wares[wareSetIndex].gameObject.GetComponent<Ware>();
        wares[wareSetIndex] = null;
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
