using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProcessType
{
    切块 = 0, 切丁 = 1, 切条 = 2
}

public class ProcessController : MonoBehaviour
{
    public GameObject SelectPanel;

    public GameObject Table;
    
    [SerializeField]
    ProcessTarget data0;
    [SerializeField]
    ProcessTarget data1;
    [SerializeField]
    ProcessTarget data2;

    private ProcessTarget targetOriginal;

    List<GameObject> originals = new List<GameObject>();

    private OriginalTypeTemp typeTemp;

    public void PushPanel()
    {
        SelectPanel.SetActive(true);
    }

    public void Init()
    {
        originals.Clear();
        Debug.Log("IN CUT");

        if (Table.GetComponentInChildren<Tomato>() != null)
        {
            Debug.Log("Cut FIND TOMATO");

            typeTemp = OriginalTypeTemp.西红柿;
            var temp = Table.GetComponentsInChildren<Tomato>();
            
            foreach(var item in temp)
            {
                originals.Add(item.gameObject);
                Debug.Log("Cut Find One:" + item.gameObject);
            }
        }
        else if(Table.GetComponentInChildren<Garlic>() != null)
        {
            typeTemp = OriginalTypeTemp.蒜;
            var temp = Table.GetComponentsInChildren<Garlic>();
            foreach (var item in temp)
            {
                originals.Add(item.gameObject);
            }
        }
        else if (Table.GetComponentInChildren<Scallion>() != null)
        {
            typeTemp = OriginalTypeTemp.葱;
            var temp = Table.GetComponentsInChildren<Scallion>();
            foreach (var item in temp)
            {
                originals.Add(item.gameObject);
            }
        }
        else
        {
            Debug.Log("Cut Null Type!");
            Quit();
        }

        if ((int)typeTemp == 0)
        {
            targetOriginal = data0;
        }
        else if ((int)typeTemp == 1)
        {
            targetOriginal = data1;
        }
        else if ((int)typeTemp == 2)
        {
            targetOriginal = data2;
        }
        else
        {
            Debug.Log("Cut Null Type!");
        }
    }

    public void Selected_0()
    {
        Debug.Log("Cut SELECT 0");
        Selected(0);
    }

    public void Selected_1()
    {
        Selected(1);
    }

    public void Selected_2()
    {
        Selected(2);
    }

    void Selected(int value)
    {
        Init();
        Debug.Log("Cut PROCESS!");
        Debug.Log("Cut Origins:" + originals);
        foreach (var item in originals)
        {
            Transform temp = item.transform.parent;
            Destroy(item.transform.gameObject);
            // for (int i = 0; i < item.transform.childCount; i++)
            // {
            //     Destroy(item.transform.GetChild(i).gameObject);
            //     Debug.Log("Cut Destroy:" + item + "[" + i +"]");
            //
            // }
            Instantiate(targetOriginal.targetPrefab[value], temp);
            Debug.Log("Cut Instance:" + item);

        }
        Quit();
    }

    public void Quit()
    {
        targetOriginal = null;
        originals.Clear();
        SelectPanel.SetActive(false);
    }
}

public enum OriginalTypeTemp
{
    西红柿, 葱, 蒜
}

[System.Serializable]
public class ProcessTarget
{
    public OriginalTypeTemp originalType;
    public GameObject[] targetPrefab = new GameObject[3];
}
