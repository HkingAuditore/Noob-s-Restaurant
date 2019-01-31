using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scallion : Ingredient
{

    //public Material leafHeatedMat;
    // public Material ballHeatedMat;



    protected override void Start()
    {
        base.Start();
        FoodName = FoodName.Scallion;
    }



    /* private void Update()
     {
         //ChangeMat();
         // Debug.Log(HeatTime);
     }*/


    /* void ChangeMat()
     {
         if (HeatTime > targetTime)
         {
             transform.Find("叶").gameObject.GetComponent<MeshRenderer>().sharedMaterial = leafHeatedMat;
             transform.Find("果").gameObject.GetComponent<MeshRenderer>().sharedMaterial = ballHeatedMat;
         }
     }*/

}