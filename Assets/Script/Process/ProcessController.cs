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

    public void Init()
    {
        originals.Clear();
        Debug.Log("IN CUT");

        if (Table.GetComponentInChildren<Tomato>() != null)
        {
            Debug.Log("FIND TOMATO");

            typeTemp = OriginalTypeTemp.西红柿;
            var temp = GetComponentsInChildren<Tomato>();
            foreach(var item in temp)
            {
                originals.Add(item.gameObject);
            }
        }
        else if(Table.GetComponentInChildren<Garlic>() != null)
        {
            typeTemp = OriginalTypeTemp.蒜;
            var temp = GetComponentsInChildren<Garlic>();
            foreach (var item in temp)
            {
                originals.Add(item.gameObject);
            }
        }
        else if (Table.GetComponentInChildren<Scallion>() != null)
        {
            typeTemp = OriginalTypeTemp.葱;
            var temp = GetComponentsInChildren<Scallion>();
            foreach (var item in temp)
            {
                originals.Add(item.gameObject);
            }
        }
        else
        {
            Debug.Log("Null Type!");
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
            Debug.Log("Null Type!");
        }
        SelectPanel.SetActive(true);
    }

    public void Selected_0()
    {
        Debug.Log("SELECT 0");
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
        Debug.Log("PROCESS!");

        foreach (var item in originals)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                Destroy(item.transform.GetChild(i).gameObject);
            }
            Instantiate(targetOriginal.targetPrefab[value], item.transform);
        }
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
