using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Bowl : Ware
{
    protected override void Start()
    {
        base.Start();
        offset = 3;
    }

    protected override void InitIngredientsList()
    {
        Contents.AddRange(GetComponentsInChildren<Ingredient>());
    }
}