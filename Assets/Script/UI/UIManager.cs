using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Canvas canvas;
    private Dictionary<string, GameObject> currentSceneUIPanelDict = new Dictionary<string, GameObject>();
    private Stack<GPObject> panelStack = new Stack<GPObject>();
    private Action mdUpdateAction;

    private struct GPObject
    {
        public GameObject panelObject;
        public IPanel panelScript;
    }

    public void Init()
    {
    }

    public void UpdateUI()
    {
        if (mdUpdateAction != null && mdUpdateAction.GetInvocationList().Length > 0)
        {
            mdUpdateAction();
        }
    }

    //获取当前场景所有UI
    public void GetCurrentSceneUIObject()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        if (currentSceneUIPanelDict.Count > 0)
            currentSceneUIPanelDict.Clear();
        if (canvas != null)
        {
            foreach (Transform temp in canvas.transform)
            {
                currentSceneUIPanelDict.Add(temp.name, temp.gameObject);
                if(temp.name!= "ConversationProcess")
                    temp.gameObject.SetActive(false);
            }
        }
    }

    public void PushPanel(IPanel panelScript)
    {
        GameObject panelObject;
        GPObject gp;

        //根据字典查找当前场景中是否存在目标ui
        if (currentSceneUIPanelDict.TryGetValue(panelScript.GetPanelName(), out panelObject))
        {
            //Debug.Log(string.Format("查找到{0},准备入栈", panelScript.GetPanelName()));
        }
        else
        {
            Debug.Log("当前场景中没有该ui");
            return;
        }

        //调用前一个panel的OnPause()
        if (panelStack.Count > 0)
        {
            GPObject temp = panelStack.Peek();
            mdUpdateAction -= temp.panelScript.OnUpdate;
            temp.panelScript.OnPause();
        }

        //打包入栈
        gp.panelScript = panelScript;
        gp.panelObject = panelObject;
        panelStack.Push(gp);

        //初始化
        gp.panelScript.OnEnter();
        mdUpdateAction += gp.panelScript.OnUpdate;
    }

    public void PopPanel()
    {
        if (panelStack.Count > 0)
        {
            //出栈
            GPObject gp = panelStack.Pop();
            mdUpdateAction -= gp.panelScript.OnUpdate;
            gp.panelScript.OnExit();

            //调用前一个panel的OnEnter()
            if (panelStack.Count > 0)
            {
                gp = panelStack.Peek();
                gp.panelScript.OnEnter();
                mdUpdateAction += gp.panelScript.OnUpdate;
            }
        }
    }

    public void PopSpecifiedQuantityOfPanel(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //出栈
            GPObject gp = panelStack.Pop();
            mdUpdateAction -= gp.panelScript.OnUpdate;
            gp.panelScript.OnExit();

            //调用前一个panel的OnEnter()
            if (panelStack.Count > 0)
            {
                gp = panelStack.Peek();
                gp.panelScript.OnEnter();
                mdUpdateAction += gp.panelScript.OnUpdate;
            }
        }
    }
}