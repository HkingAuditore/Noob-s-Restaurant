using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato : MonoBehaviour,IFood {

    public FoodName GetFoodName()
    {
        return FoodName.Tomato;
    }
}
