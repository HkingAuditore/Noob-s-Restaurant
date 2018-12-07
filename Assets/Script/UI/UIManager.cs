using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UIManager {

    Canvas Canvas;
    Dictionary<string,GameObject> CurrentSceneUIPanelDict = new Dictionary<string, GameObject>();
    Stack<GameObject> PanelStack = new Stack<GameObject>();

    //每次切换场景后调用
    public void Init()
    {
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        GetCurrentSceneUIObject();
    }

    //获取当前场景所有UI
    public void GetCurrentSceneUIObject()
    {
        if (CurrentSceneUIPanelDict.Count > 0)
            CurrentSceneUIPanelDict.Clear();
        if (Canvas != null)
        {
            foreach (Transform temp in Canvas.transform)
            {
                CurrentSceneUIPanelDict.Add(temp.name, temp.gameObject);
                temp.gameObject.SetActive(false);
                Debug.Log("当前场景UI：\n" + temp.name);
            }
        }
    }

    public void PushPanel(string uiPanelName)
    {
        GameObject UIPanel;
        if (CurrentSceneUIPanelDict.TryGetValue(uiPanelName, out UIPanel))
        {
            Debug.Log(string.Format("查找到{0},准备入栈", UIPanel.name));
        }
        else
        {
            Debug.Log("当前场景中没有该ui");
            return;
        }

        if (PanelStack.Count > 0)
        {
            GameObject PrePanel = PanelStack.Peek();
            PrePanel.GetComponent<IPanel>().OnPause();
        }

        PanelStack.Push(UIPanel);
        UIPanel.GetComponent<IPanel>().OnEnter();

        Debug.Log("当前栈顶：" + PanelStack.Peek().name);
    }

    public void PopPanel()
    {
        GameObject UIPanel;
        if (PanelStack.Count > 0)
        {
            UIPanel = PanelStack.Pop();
            UIPanel.GetComponent<IPanel>().OnExit();
            if (PanelStack.Count > 0)
            {
                UIPanel = PanelStack.Peek();
                UIPanel.GetComponent<IPanel>().OnEnter();
            }
        }
        Debug.Log("当前栈顶：" + PanelStack.Peek().name);
    }
}
