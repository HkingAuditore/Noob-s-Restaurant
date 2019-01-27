using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Ingredient
{
    protected override void Start()
    {
        base.Start();
        FoodName = FoodName.Egg;
    }
}
