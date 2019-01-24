using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBlock : Utensil
{
    protected override void Awake()
    {
        base.Awake();

        DropFoodPos = transform.Find("DropFoodPos").position;
    }
}
