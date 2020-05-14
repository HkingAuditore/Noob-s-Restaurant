using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PTable : Table
{
    [SerializeField]
    GameObject pCameraGo;
    GameObject crackGo;
    EggBowl eggBowl;
    Animator crackAnimator;

    protected override void Awake()
    {
        base.Awake();
        eggBowl = transform.Find("UtensilSet").gameObject.GetComponentInChildren<EggBowl>();
        crackGo = eggBowl.transform.Find("Bowl/Crack").gameObject;
        crackAnimator = crackGo.GetComponent<Animator>();
    }

    protected override void Start()
    {
        Name = TableName.P;
        thisRowMaxPlaceNum = 1;
        thisMaxPlaceNum = 2;
        thisColumnFoodSetSpace = -1.5f;
        thisRowFoodSetSpace = 0f;
        wares = new List<Container>(thisMaxPlaceNum);
        crackGo.SetActive(false);
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!isEnter)
            return;

        PutEggToEggBowl();
        PutCrackedEggToCurrentChosenWare();
    }



    private void PutEggToEggBowl()
    {
        if (crackGo.activeSelf && crackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            crackGo.SetActive(false);
            crackAnimator.SetBool("isCrack", false);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentChosenWare != null && currentChosenWare.Contents.Count > 0)
            {
                if (crackAnimator.GetBool("isCrack") == false
                    && currentChosenWare.Contents[0].FoodName == FoodName.Egg
                    && eggBowl.IsBeatingEgg
                    && eggBowl.IsInPlace)
                {
                    crackGo.SetActive(true);
                    crackAnimator.SetBool("isCrack", true);
                    Ingredient chosenEgg = currentChosenWare.Contents[Random.Range(0, currentChosenWare.Contents.Count)];
                    currentChosenWare.TakeOneTo(chosenEgg, eggBowl);
                    chosenEgg.GetComponent<Egg>().EggState = EggState.NoStir;
                }
            }
        }
    }

    //table 实现的 IContainable 的一些方法不太适用于破壳后的鸡蛋，这里暂时另写单独的方法控制取出
    void PutCrackedEggToCurrentChosenWare()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (currentChosenWare.Contents.Count == 0 && eggBowl.Contents.Count > 0)
            {
                currentChosenWare.Contents.AddRange(eggBowl.Contents);
                GameObject containedEgg = eggBowl.transform.Find("Bowl/ContainedEgg").gameObject;
                GameObject wareContainedEgg = GameObject.Instantiate(containedEgg, currentChosenWare.transform);
                wareContainedEgg.name = "ContainedEgg";
                foreach (Ingredient temp in eggBowl.Contents)
                {
                    temp.gameObject.SetActive(false);
                    temp.transform.localPosition = Vector3.zero;
                    temp.transform.parent = currentChosenWare.transform;
                }
                eggBowl.ResetEggBowl();
            }
        }
    }

    protected override void GetCamera()
    {
        cameraGO = pCameraGo;
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
        PutWareOnTablePreelectionPos();
        GameManager.Instance.uiManager.PushPanel(new PTableHintPanel());
    }

    protected override void OnQuitTable()
    {
        base.OnQuitTable();
        GivePlayerSelectedWare();
        GameManager.Instance.uiManager.PopPanel();
    }

    [ContextMenu("ResetWaresPos")]
    public void ResetWaresPos()
    {
        Awake();

        ResetWaresPos(thisRowMaxPlaceNum, thisColumnFoodSetSpace, thisRowFoodSetSpace);
    }
}
