﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Bowl : Ware
{
    protected override void Start()
    {
        base.Start();

        dropOffset = 0.05f;
    }

    protected override void InitIngredientsList()
    {
        Contents.AddRange(GetComponentsInChildren<Ingredient>());
    }
}