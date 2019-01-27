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
        crackGo = eggBowl.transform.Find("Crack").gameObject;
        crackAnimator = crackGo.GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        thisRowMaxPlaceNum = 1;
        thisMaxPlaceNum = 2;
        thisColumnFoodSetSpace = -4.28f;
        thisRowFoodSetSpace = 0f;
        wares = new List<Container>(thisMaxPlaceNum);
        crackGo.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        PutEggToEggBowl();
    }

    private void PutEggToEggBowl()
    {
        if (crackGo.activeSelf&& crackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0)
        {
            crackAnimator.SetBool("isCrack", false);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentChosenWare != null && currentChosenWare.Contents.Count > 0 )
            {
                if (crackAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    crackAnimator.SetBool("isCrack", false);                    

                if (crackAnimator.GetBool("isCrack") == false && currentChosenWare.Contents[0].FoodName == FoodName.Egg)
                {                    
                    crackGo.SetActive(true);
                    crackAnimator.SetBool("isCrack", true);
                    Ingredient chosenEgg = currentChosenWare.Contents[Random.Range(0, currentChosenWare.Contents.Count)];
                    currentChosenWare.TakeOneTo(chosenEgg, eggBowl);
                }
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
    }

    protected override void OnQuitTable()
    {
        base.OnQuitTable();
        GivePlayerSelectedWare();
    }

    [ContextMenu("ResetWaresPos")]
    public void ResetWaresPos()
    {
        Awake();

        ResetWaresPos(thisRowMaxPlaceNum, thisColumnFoodSetSpace, thisRowFoodSetSpace);
    }
}
