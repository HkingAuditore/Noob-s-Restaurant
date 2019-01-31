using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Table : MonoBehaviour, IContainable<Container>
{
    public Vector3 DropFoodPos { get; set; }

    protected PlayerCtrl playerCtrl;
    protected GameObject cameraGO;
    protected List<Tool> tools;
    protected List<Container> wares;
    protected List<Utensil> utensils;
    protected bool isEnter;

    public event Action<AttentionType, object> QuitTableEvent;

    public Timer HeatTimer { get; protected set; }

    //食物选择相关
    protected Ware currentChosenWare;//存储选择完成后被选中的 foodSet
    protected GameObject cBowlGo;//需要点亮的碗
    protected Coroutine selectFoodSetCoroutine;
    protected int wareSetIndex;
    protected Material[] outLineMs;
    protected Material[] defaultMs;

    protected Transform wareSetTrans;
    protected Transform preelectionFoodSetTrans;
    protected Transform _11MarkTrans;

    //[SerializeField]
    protected int thisRowMaxPlaceNum = 0;//控制此桌一排的最大放碗数
    //[SerializeField]
    protected int thisMaxPlaceNum = 0;//控制此桌最大容量
    //[SerializeField]
    protected float thisColumnFoodSetSpace = 0;//用于设置桌子上foodset间的列间隔 
    //[SerializeField]
    protected float thisRowFoodSetSpace = 0;//用于设置桌子上foodset间的行间隔

    public List<Container> Contents
    {
        get
        {
            return wares;
        }
    }

    protected virtual void Awake()
    {
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        tools = new List<Tool>();
        utensils = new List<Utensil>();
        //wareSet = new List<Container>();
        //utensil = new List<Utensil>();
        //outLineMs = new Material[2] { new Material(Shader.Find("Custom/Outline")), new Material(Shader.Find("Custom/Outline")) };
        //defaultMs = new Material[2] { new Material(Shader.Find("Standard (Specular setup)")), new Material(Shader.Find("Standard (Specular setup)")) };
        outLineMs = new Material[2] { Resources.Load<Material>("Materials/BowlOutline"), Resources.Load<Material>("Materials/BowlOutline") };
        defaultMs = new Material[2] { Resources.Load<Material>("Materials/Bowl"), Resources.Load<Material>("Materials/Bowl") };
        preelectionFoodSetTrans = transform.Find("PreelectionFoodSetMark");
        wareSetTrans = transform.Find("WareSet");
        _11MarkTrans = transform.Find("11Mark");
    }

    protected virtual void Start()
    {
        GetWareSet();
        GetToolSet();
        GetUtensilSet();
        GetCamera();

        SwitchCamera(false);

        QuitTableEvent += GameManager.Instance.sequenceManager.StepTriggerHandler;
    }

    protected virtual void Update()
    {

        if (HeatTimer != null)
            HeatTimer.Update(Time.deltaTime);
        if (!isEnter)
            return;
    }

    protected void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || cameraGO == null)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !isEnter)
        {
            OnEnterTable();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isEnter)
        {
            OnQuitTable();
        }

        if (isEnter)
            SelectFoodSet();
    }

    protected void GetWareSet()
    {
        if (this.transform.Find("WareSet") != null && wares != null)
        {
            wares.AddRange(this.transform.Find("WareSet").GetComponentsInChildren<Ware>());
        }
    }

    protected void GetToolSet()
    {
        if (this.transform.Find("ToolSet") != null && tools != null)
        {
            tools.AddRange(this.transform.Find("ToolSet").gameObject.GetComponentsInChildren<Tool>());
        }
    }

    protected void GetUtensilSet()
    {
        if (this.transform.Find("UtensilSet") != null && utensils != null)
        {
            utensils.AddRange(this.transform.Find("UtensilSet").gameObject.GetComponentsInChildren<Utensil>());
        }
    }

    protected abstract void GetCamera();

    protected void SwitchCamera(bool isActive)
    {
        cameraGO.SetActive(isActive);
    }

    protected void SetToolGo(bool isBeginCtrl)
    {
        if (tools != null && tools.Count > 0)
        {
            foreach (Tool temp in tools)
            {
                if (isBeginCtrl)
                    temp.BeginCtrl();
                else
                    temp.StopCtrl();
            }
        }
    }

    protected void SetUtensilGo(bool isBeginCtrl)
    {
        if (utensils != null && utensils.Count > 0)
        {
            foreach (Utensil temp in utensils)
            {
                if (isBeginCtrl)
                    temp.BeginCtrl();
                else
                    temp.StopCtrl();
            }
        }
    }

    protected void SetPlayerCtrl(bool isShow)
    {
        if (isShow)
        {
            playerCtrl.Show();
            playerCtrl.isCanCtrl = true;
        }
        else
        {
            playerCtrl.Hide();
            playerCtrl.isCanCtrl = false;
        }
    }

    protected virtual void OnQuitTable()
    {
        //QuitTableInit
        SwitchCamera(false);
        SetToolGo(false);
        SetUtensilGo(false);
        SetPlayerCtrl(true);

        if (selectFoodSetCoroutine != null)
        {
            cBowlGo.GetComponent<Renderer>().materials = defaultMs;
            StopCoroutine("SelectFoodSetCoroutine");
            selectFoodSetCoroutine = null;
        }
        isEnter = false;

        if (currentChosenWare != null)
            if (QuitTableEvent != null)
                QuitTableEvent(AttentionType.QuitTable, null);
    }

    protected virtual void OnEnterTable()
    {
        //EnterTableInit
        SwitchCamera(true);
        SetToolGo(true);
        SetUtensilGo(true);
        SetPlayerCtrl(false);
        isEnter = true;
        Debug.Log(Contents.Count);
    }

    protected virtual void SelectFoodSet()
    {
        if (selectFoodSetCoroutine == null && wares != null && !playerCtrl.isCanCtrl)
        {
            if (Input.GetMouseButtonDown(2) && wares.Count > 0)
            {
                selectFoodSetCoroutine = StartCoroutine("SelectFoodSetCoroutine");
            }
        }
    }

    IEnumerator SelectFoodSetCoroutine()
    {
        yield return null;

        wareSetIndex = 0;
        while (wares[wareSetIndex] == null)
            wareSetIndex++;

        cBowlGo = wares[wareSetIndex].transform.Find("BowlModel/球体").gameObject;

        while (true)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                wareSetIndex++;
                if (wareSetIndex >= wares.Count)
                {
                    wareSetIndex = 0;
                }

                if (wares[wareSetIndex] == null)
                {
                    wareSetIndex++;
                }

                if (wareSetIndex >= wares.Count)
                {
                    wareSetIndex = 0;
                }
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                wareSetIndex--;
                if (wareSetIndex <= -1)
                {
                    wareSetIndex = wares.Count - 1;
                }

                if (wares[wareSetIndex] == null)
                {
                    wareSetIndex--;
                }

                if (wareSetIndex <= -1)
                {
                    wareSetIndex = wares.Count - 1;
                }
            }

            //设置高亮    
            cBowlGo.GetComponent<Renderer>().materials = defaultMs;
            cBowlGo = wares[wareSetIndex].transform.Find("BowlModel/球体").gameObject;
            cBowlGo.GetComponent<Renderer>().materials = outLineMs;

            if (Input.GetMouseButtonDown(2))
            {
                SelectMethod();//每个桌子选择后处理方式不同可重写此方法
                cBowlGo.GetComponent<Renderer>().materials = defaultMs;
                StopCoroutine("SelectFoodSetCoroutine");
                selectFoodSetCoroutine = null;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("取消");
                CancelMethod();//每个桌子选择后处理方式不同可重写此方法
                cBowlGo.GetComponent<Renderer>().materials = defaultMs;
                StopCoroutine("SelectFoodSetCoroutine");
                selectFoodSetCoroutine = null;
            }
            yield return null;
        }
    }


    protected void PutFoodSetBack()
    {
        if (currentChosenWare != null)
        {
            int i;
            for (i = 0; i < thisMaxPlaceNum; i++)
            {
                if ((i >= wares.Count && i < wares.Capacity))
                    wares.Add(null);
                if (wares[i] == null)
                {
                    //Debug.Log(i + "号位空缺");
                    //Debug.Log((i * thisRowFoodSetSpace));

                    if (i >= thisRowMaxPlaceNum)
                    {
                        currentChosenWare.transform.localPosition =
                            new Vector3(_11MarkTrans.localPosition.x - ((i - thisRowMaxPlaceNum) * thisRowFoodSetSpace), _11MarkTrans.localPosition.y, _11MarkTrans.localPosition.z + thisColumnFoodSetSpace);
                    }
                    else
                    {
                        currentChosenWare.transform.localPosition =
                            new Vector3(_11MarkTrans.localPosition.x - (i * thisRowFoodSetSpace), _11MarkTrans.localPosition.y, _11MarkTrans.localPosition.z);
                    }
                    wares[i] = currentChosenWare;
                    currentChosenWare = null;
                    break;
                }
            }
            if (i > thisRowMaxPlaceNum)
            {
                Debug.Log("此桌已经没有空位了");
                return;
            }
        }
    }

    protected void PutWareOnTablePreelectionPos()
    {
        if (playerCtrl.isHoldFoodSet)
        {
            playerCtrl.TakeTheOneTo(this);
        }
    }

    protected void GivePlayerSelectedWare()
    {
        if (playerCtrl.isHoldFoodSet)
        {
            Debug.Log("已经持有" + playerCtrl.Contents[0].name);
            return;
        }

        TakeTheOneTo(playerCtrl);
    }

    //每个桌子选择后处理方式不同可重写此方法
    protected virtual void SelectMethod() { }
    //每个桌子选择后处理方式不同可重写此方法 
    protected virtual void CancelMethod() { }

    protected virtual void ResetWaresPos(int rowMaxPlaceNum, float columnFoodSetSpace, float rowFoodSetSpace)
    {
        _11MarkTrans = transform.Find("11Mark");
        List<Container> l = new List<Container>();
        l.AddRange(transform.Find("WareSet").GetComponentsInChildren<Ware>());
        for (int i = 0; i < l.Count; i++)
        {
            int indexZ = i % rowMaxPlaceNum;
            int indexX = i / rowMaxPlaceNum;
            l[i].transform.localPosition = new Vector3(
                _11MarkTrans.transform.localPosition.x - indexZ * rowFoodSetSpace,
                _11MarkTrans.transform.localPosition.y,
                _11MarkTrans.transform.localPosition.z + indexX * columnFoodSetSpace);
        }
    }

    #region IContainable Implement
    public void AddToContents(Container ware)
    {
        //Contents.Add(ware);
        ware.transform.position = preelectionFoodSetTrans.position;
        ware.transform.SetParent(wareSetTrans);
        currentChosenWare = ware as Ware;
    }

    public Container TakeTheOneTo(IContainable<Container> container)
    {
        if (currentChosenWare == null)
        {
            Debug.Log("Nothing chosen to take");
            return null;
        }
        //if (!Contents.Contains(currentChosenWare))
        //{
        //    Debug.LogError("the given content does not exist in this container");
        //    return null;
        //}

        Ware ware = currentChosenWare;
        container.AddToContents(ware);
        Contents.Remove(ware);
        currentChosenWare = null;
        return ware;
    }

    public Container TakeOneTo(Container ware, IContainable<Container> container)
    {
        throw new System.NotImplementedException();
    }
    public void AddRangeToContents(List<Container> ingredient, Vector3 posOffset)
    {
        throw new System.NotImplementedException();
    }
    public List<Container> TakeOutAllTo(IContainable<Container> container)
    {
        throw new System.NotImplementedException();
    }
    #endregion

}
