using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato : Ingredient
{
    protected override void Start()
    {
        base.Start();
        FoodName = FoodName.Tomato;
    }
}
