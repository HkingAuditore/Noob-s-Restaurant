using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Ingredient
{
    EggState eggState;

    public EggState EggState
    {
        get
        {
            return eggState;
        }

        set
        {
            eggState = value;
        }
    }

    protected override void Start()
    {
        base.Start();
        FoodName = FoodName.Egg;
        eggState = EggState.WithShell;
    }

    private void Update()
    {
        
    }
}

