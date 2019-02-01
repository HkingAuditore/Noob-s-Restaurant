using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SFTable : Table
{
    [SerializeField]
    GameObject sfCamera;
    [SerializeField]
    GameObject endCamera;
    Pan pan;

    protected override void Awake()
    {
        base.Awake();

        thisRowMaxPlaceNum = 2;
        thisMaxPlaceNum = 4;
        thisColumnFoodSetSpace = 4.37f;
        thisRowFoodSetSpace = -1.4f;
        Name = TableName.SF;
    }

    protected override void Start()
    {
        wares = new List<Container>(thisMaxPlaceNum);
        GetUtensil();

        base.Start();

        HeatTimer = new Timer(new float[] { 30, 60, 90 }, false);
        HeatTimer.onUpdate += OnHeatUpdate;
    }

    protected override void Update()
    {
        base.Update();

        if (!isEnter)
            return;

        if (Input.GetKeyDown(KeyCode.P))
            PutIngredientsFromCurChosenWareToPan();

        if (Input.GetKeyDown(KeyCode.O))
            PutIngredientsFromPanToCurChosenWare();

        if (endCamera.activeInHierarchy)
            endCamera.transform.localEulerAngles = new Vector3(endCamera.transform.localEulerAngles.x, endCamera.transform.localEulerAngles.y, endCamera.transform.localEulerAngles.z + 2f);
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
        HeatTimer.onUpdate -= OnHeatUpdate;
    }

    private void PutIngredientsFromCurChosenWareToPan()
    {
        if (currentChosenWare != null)
        {
            currentChosenWare.TakeOutAllTo(pan);

            if (currentChosenWare.transform.Find("ContainedEgg") != null)
            {
                currentChosenWare.transform.Find("ContainedEgg").position = pan.DropFoodPos - Vector3.up * 0.2f;
                currentChosenWare.transform.Find("ContainedEgg").SetParent(pan.transform);
            }
        }
    }

    public void PushEndPanel()
    {
        GameManager.Instance.uiManager.PushPanel(new EndPanel());
        Cursor.visible = true;
    }

    private void PutIngredientsFromPanToCurChosenWare()
    {
        //if (GameManager.Instance.sequenceManager.IsCurSeqOver)
        {
            if (currentChosenWare is Dish)
            {
                currentChosenWare.Contents.ForEach((ingredient) => Destroy(ingredient));
                currentChosenWare.gameObject.SetActive(false);
                GameObject dish = Instantiate(Resources.Load<GameObject>("Prefabs/StiredEggAndTomato"));
                dish.transform.position = currentChosenWare.transform.position; 

                endCamera.SetActive(true);

                Invoke("PushEndPanel", 5);
            }
        }
        //else
        if (currentChosenWare != null && pan.Contents.Count > 0)
        {
            Ingredient ingredient = pan.Contents[Random.Range(0, pan.Contents.Count)];
            pan.TakeOneTo(ingredient, currentChosenWare);
            Debug.Log(ingredient.HeatTime);
        }
    }

    private void OnHeatUpdate(float deltaTime)
    {
        pan.Contents.ForEach((ingredient) => ingredient.UpdateHeatTime(deltaTime));
    }
    //private void OnHeatStop(float onceTime, float sumTime)
    //{
    //    pan.Contents.ForEach((Ingredient) => Ingredient.UpdateHeatTime(onceTime));
    //}

    protected override void GetCamera()
    {
        cameraGO = sfCamera;
    }

    private void GetUtensil()
    {
        if (transform.Find("UtensilSet") != null)
        {
            pan = transform.Find("UtensilSet").gameObject.GetComponentInChildren<Pan>();
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
        GameManager.Instance.uiManager.PushPanel(new SFTableHintPanel());
        GameManager.Instance.uiManager.PushPanel(new CookStatePanel());
    }

    protected override void OnQuitTable()
    {
        base.OnQuitTable();

        SetUtensilState(false);
        GivePlayerSelectedWare();
        GameManager.Instance.uiManager.PopSpecifiedQuantityOfPanel(2);
    }

    private void SetUtensilState(bool isBeginCtrl)
    {
        if (pan != null)
        {
            if (isBeginCtrl)
                pan.BeginCtrl();
            else
                pan.StopCtrl();
        }
    }

    [ContextMenu("ResetWaresPos")]
    public void ResetWaresPos()
    {
        Awake();

        ResetWaresPos(thisRowMaxPlaceNum, thisColumnFoodSetSpace, thisRowFoodSetSpace);
    }
}
