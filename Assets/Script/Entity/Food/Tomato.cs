using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato : Ingredient
{

    public Material leafHeatedMat;
    public Material ballHeatedMat;



    protected override void Start()
    {
        base.Start();
        FoodName = FoodName.Tomato;
    }



    private void Update()
    {
        ChangeMat();
        //Debug.Log(HeatTime);
    }


    void ChangeMat()
    {
        if (HeatTime > targetTime)
        {
            transform.Find("叶").gameObject.GetComponent<MeshRenderer>().material = leafHeatedMat;
            transform.Find("果").gameObject.GetComponent<MeshRenderer>().material = ballHeatedMat;
        }
    }

}
