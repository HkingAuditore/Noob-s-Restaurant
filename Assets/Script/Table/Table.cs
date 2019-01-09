using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Table : MonoBehaviour
{

    protected PlayerCtrl playerCtrl;
    protected GameObject cameraGO;
    protected List<Tool> toolSet;
    protected List<Container> wareSet;
    protected bool isEnter;

    //食物选择相关
    protected Ware currentChosenWare;//存储选择完成后被选中的 foodSet
    protected GameObject cBowl;//需要点亮的碗
    protected Coroutine selectFoodSetCoroutine;
    protected int wareSetIndex;
    protected Material[] outLineMs;
    protected Material[] defaultMs;

    protected virtual void Awake()
    {
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        toolSet = new List<Tool>();
        wareSet = new List<Container>();
        outLineMs = new Material[2] { new Material(Shader.Find("Custom/Outline")), new Material(Shader.Find("Custom/Outline")) };
        defaultMs = new Material[2] { new Material(Shader.Find("Standard (Specular setup)")), new Material(Shader.Find("Standard (Specular setup)")) };
    }

    protected virtual void Start()
    {
        GetWareSet();
        GetToolSet();
        GetCamera();

        SwitchCamera(false);
    }

    protected void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || cameraGO == null)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !isEnter)
        {
            //EnterTableInit
            SwitchCamera(true);
            SetToolGo(true);
            SetPlayerCtrl(false);
            OnEnterTable();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isEnter)
        {
            //QuitTableInit
            SwitchCamera(false);
            SetToolGo(false);
            SetPlayerCtrl(true);
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
        if (toolSet != null && toolSet.Count>0)
        {
            foreach (Tool temp in toolSet)
            {
                if (isBeginCtrl)
                    temp.OnBeginCtrl();
                else
                    temp.OnStopCtrl();
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
        isEnter = true;
    }

    protected virtual void SelectFoodSet()
    {
        if (selectFoodSetCoroutine == null && wareSet !=null && !playerCtrl.isCanCtrl)
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
        if (wareSet[wareSetIndex] == null)
        {
            wareSetIndex++;
        }
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

    //每个桌子选择后处理方式不同可重写此方法
    protected virtual void SelectMethod() { }
    //每个桌子选择后处理方式不同可重写此方法 
    protected virtual void CancelMethod() { }
}
