using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SMTable : Table, IContainer<Container>
{
    [SerializeField]
    private GameObject smCamera;
    [SerializeField]
    private GameObject toolSet;
    private Transform wareSetTrans;
    private Transform preelectionFoodSetTrans;
    private Transform _11MarkTrans;
    [SerializeField]
    private int thisRowMaxPlaceNum = 5;//控制此桌一排的最大放碗数
    [SerializeField]
    private int thisMaxPlaceNum = 10;//控制此桌最大容量
    [SerializeField]
    private float thisColumnFoodSetSpace = 0.63f;//用于设置桌子上foodset间的列间隔 
    [SerializeField]
    private float thisRowFoodSetSpace = 0.57f;//用于设置桌子上foodset间的行间隔

    public List<Container> Contents
    {
        get { return wareSet; }
    }

    protected override void Awake()
    {
        base.Awake();
        preelectionFoodSetTrans = this.transform.Find("PreelectionFoodSetMark");
        wareSetTrans = this.transform.Find("WareSet");
        _11MarkTrans = this.transform.Find("11Mark");
    }

    protected override void Start()
    {
        base.Start();
        thisCameraGO = smCamera;
        toolGo = toolSet;
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

    private void PutFoodSetBack()
    {
        if (currentChosenWare != null)
        {
            int i;
            for (i = 0; i < thisMaxPlaceNum; i++)
            {
                if (wareSet[i] == null)
                {
                    Debug.Log(i + "号位孔雀");
                    Debug.Log((i * thisRowFoodSetSpace));

                    if (i >= thisRowMaxPlaceNum)
                    {
                        currentChosenWare.transform.localPosition =
                            new Vector3(_11MarkTrans.localPosition.x - thisColumnFoodSetSpace, _11MarkTrans.localPosition.y, _11MarkTrans.localPosition.z - ((i - thisRowMaxPlaceNum) * thisRowFoodSetSpace));
                    }
                    else
                    {
                        currentChosenWare.transform.localPosition =
                            new Vector3(_11MarkTrans.localPosition.x, _11MarkTrans.localPosition.y, _11MarkTrans.localPosition.z - (i * thisRowFoodSetSpace));
                    }
                    wareSet[i] = currentChosenWare.GetComponent<Ware>();
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

    private void PutWareOnTablePreelectionPos()
    {
        if (playerCtrlScript.isHoldFoodSet)
        {
            playerCtrlScript.TakeTheOneTo(this);
        }
    }

    private void GivePlayerSelectedWare()
    {
        if (playerCtrlScript.isHoldFoodSet)
        {
            Debug.Log("已经持有" + playerCtrlScript.Contents[0].name);
            return;
        }

        TakeTheOneTo(playerCtrlScript);
    }

    //IContainer Implement
    public void Add(Container ware)
    {
        Contents.Add(ware);
        ware.transform.position = preelectionFoodSetTrans.position;
        ware.transform.SetParent(wareSetTrans);
        currentChosenWare = ware as Ware;
    }
    public Container TakeTheOneTo(IContainer<Container> container)
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

    public Container TakeOneTo(Container ware, IContainer<Container> container)
    {
        throw new System.NotImplementedException();
    }
    public void AddRange(List<Container> ingredient)
    {
        throw new System.NotImplementedException();
    }
    public List<Container> TakeOutAllTo(IContainer<Container> container)
    {
        throw new System.NotImplementedException();
    }

}
