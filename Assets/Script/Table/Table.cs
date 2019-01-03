using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{

    protected PlayerCtrl playerCtrlScript;
    protected GameObject toolGo;
    protected GameObject thisCameraGO;
    protected bool isEnter;

    //食物选择相关
    protected List<Container> wareSet;//每个set都会挂有该脚本，表示一碗食材 foodSet（内部预定有方法获取所装食材）
    [SerializeField]
    protected Ware currentChosenWare;//存储选择完成后被选中的 foodSet
    protected GameObject cBowl;//需要点亮的碗
    protected Coroutine selectFoodSetCoroutine;
    protected int wareSetIndex;
    protected Material[] outLineMs;
    protected Material[] defaultMs;

    protected virtual void Awake()
    {
        playerCtrlScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        outLineMs = new Material[2] { new Material(Shader.Find("Custom/Outline")), new Material(Shader.Find("Custom/Outline")) };
        defaultMs = new Material[2] { new Material(Shader.Find("Standard (Specular setup)")), new Material(Shader.Find("Standard (Specular setup)")) };
    }

    protected virtual void Start()
    {
        InitfoodSetScripts();
        if (thisCameraGO != null)
            thisCameraGO.SetActive(false);
    }

    private void Update()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || thisCameraGO == null)
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

    private void SetToolGo(bool isBeginCtrl)
    {
        if (toolGo != null && toolGo.GetComponent<ToolCtrl>() != null)
        {
            if (isBeginCtrl)
                toolGo.GetComponent<ToolCtrl>().BeginCtrl();
            else
                toolGo.GetComponent<ToolCtrl>().StopCtrl();
        }
    }

    private void SetPlayerCtrl(bool isShow)
    {
        if (isShow)
        {
            playerCtrlScript.Show();
            playerCtrlScript.isCanCtrl = true;
        }
        else
        {
            playerCtrlScript.Hide();
            playerCtrlScript.isCanCtrl = false;
        }
    }

    private void SwitchCamera(bool isActive)
    {
        thisCameraGO.SetActive(isActive);
    }

    private void InitfoodSetScripts()
    {
        if (this.transform.Find("WareSet") != null && wareSet == null)
        {
            wareSet = new List<Container>();
            wareSet.AddRange(this.transform.Find("WareSet").GetComponentsInChildren<Ware>());
        }
    }

    protected virtual void SelectFoodSet()
    {
        if (selectFoodSetCoroutine == null)
        {
            if (Input.GetMouseButtonDown(2) && !playerCtrlScript.isCanCtrl && wareSet.Count > 0)
            {
                selectFoodSetCoroutine = StartCoroutine("SelectFoodSetCoroutine");
            }
        }
    }

    IEnumerator SelectFoodSetCoroutine()
    {
        yield return new WaitForEndOfFrame();

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

    //进入与退出处理派生在这里重写
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
}
