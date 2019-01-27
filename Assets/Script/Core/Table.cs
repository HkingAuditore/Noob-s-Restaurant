using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Table : MonoBehaviour, IContainable<Container>
{
    public Vector3 DropFoodPos { get; set; }

    protected PlayerCtrl playerCtrl;
    protected GameObject cameraGO;
    protected List<Tool> toolSet;
    protected List<Container> wareSet;
    protected bool isEnter;

    public Timer HeatTimer { get; protected set; }

    //食物选择相关
    protected Ware currentChosenWare;//存储选择完成后被选中的 foodSet
    protected GameObject cBowl;//需要点亮的碗
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
            return wareSet;
        }
    }

    protected virtual void Awake()
    {
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        toolSet = new List<Tool>();
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
        GetCamera();

        SwitchCamera(false);
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

        SelectFoodSet();
    }

    protected void GetWareSet()
    {
        if (this.transform.Find("WareSet") != null && wareSet != null)
        {
            wareSet.AddRange(this.transform.Find("WareSet").GetComponentsInChildren<Ware>());
        }
    }

    protected void GetToolSet()
    {
        if (this.transform.Find("ToolSet") != null && toolSet != null)
        {
            toolSet.AddRange(this.transform.Find("ToolSet").gameObject.GetComponentsInChildren<Tool>());
        }
    }

    protected abstract void GetCamera();

    protected void SwitchCamera(bool isActive)
    {
        cameraGO.SetActive(isActive);
    }

    protected void SetToolGo(bool isBeginCtrl)
    {
        if (toolSet != null && toolSet.Count > 0)
        {
            foreach (Tool temp in toolSet)
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
        SetPlayerCtrl(true);

        if (selectFoodSetCoroutine != null)
        {
            cBowl.GetComponent<Renderer>().materials = defaultMs;
            StopCoroutine("SelectFoodSetCoroutine");
            selectFoodSetCoroutine = null;
        }
        isEnter = false;
    }

    protected virtual void OnEnterTable()
    {
        //EnterTableInit
        SwitchCamera(true);
        SetToolGo(true);
        SetPlayerCtrl(false);
        isEnter = true;
    }

    protected virtual void SelectFoodSet()
    {
        if (selectFoodSetCoroutine == null && wareSet != null && !playerCtrl.isCanCtrl)
        {
            if (Input.GetMouseButtonDown(2) && wareSet.Count > 0)
            {
                selectFoodSetCoroutine = StartCoroutine("SelectFoodSetCoroutine");
            }
        }
    }

    IEnumerator SelectFoodSetCoroutine()
    {
        yield return null;

        wareSetIndex = 0;
        while (wareSet[wareSetIndex] == null)
            wareSetIndex++;

        cBowl = wareSet[wareSetIndex].transform.Find("BowlModel/球体").gameObject;

        while (true)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                wareSetIndex++;
                if (wareSetIndex >= wareSet.Count)
                {
                    wareSetIndex = 0;
                }

                if (wareSet[wareSetIndex] == null)
                {
                    wareSetIndex++;
                }

                if (wareSetIndex >= wareSet.Count)
                {
                    wareSetIndex = 0;
                }
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                wareSetIndex--;
                if (wareSetIndex <= -1)
                {
                    wareSetIndex = wareSet.Count - 1;
                }

                if (wareSet[wareSetIndex] == null)
                {
                    wareSetIndex--;
                }

                if (wareSetIndex <= -1)
                {
                    wareSetIndex = wareSet.Count - 1;
                }
            }

            //设置高亮    
            cBowl.GetComponent<Renderer>().materials = defaultMs;
            cBowl = wareSet[wareSetIndex].transform.Find("BowlModel/球体").gameObject;
            cBowl.GetComponent<Renderer>().materials = outLineMs;

            if (Input.GetMouseButtonDown(2))
            {
                SelectMethod();//每个桌子选择后处理方式不同可重写此方法
                cBowl.GetComponent<Renderer>().materials = defaultMs;
                StopCoroutine("SelectFoodSetCoroutine");
                selectFoodSetCoroutine = null;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("取消");
                CancelMethod();//每个桌子选择后处理方式不同可重写此方法
                cBowl.GetComponent<Renderer>().materials = defaultMs;
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
                if ((i >= wareSet.Count && i < wareSet.Capacity))
                    wareSet.Add(null);

                if (wareSet[i] == null)
                {
                    Debug.Log(i + "号位空缺");
                    Debug.Log((i * thisRowFoodSetSpace));

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
                    wareSet[i] = currentChosenWare;
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

    //IContainable Implement
    public void Add(Container ware)
    {
        Contents.Add(ware);
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
        container.Add(ware);
        Contents.Remove(ware);
        currentChosenWare = null;
        return ware;
    }

    public Container TakeOneTo(Container ware, IContainable<Container> container)
    {
        throw new System.NotImplementedException();
    }
    public void AddRange(List<Container> ingredient, Vector3 posOffset)
    {
        throw new System.NotImplementedException();
    }
    public List<Container> TakeOutAllTo(IContainable<Container> container)
    {
        throw new System.NotImplementedException();
    }
}
