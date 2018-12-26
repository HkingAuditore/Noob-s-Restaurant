using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSet : MonoBehaviour {

    //持有自身拥有的所有食物
    List<IFood> foods = new List<IFood>();
    Transform FallingFoodsTrans;

    private void Awake()
    {        
        FallingFoodsTrans = GameObject.Find("Environment/Inside/FallingFoods").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IFood>() != null)
        {
            AddFoodIntoFoodsList(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IFood>() != null)
        {
            RemoveFoodFromFoodsList(other);            
        }
    }

    void AddFoodIntoFoodsList(Collider other)
    {
        IFood food = other.GetComponent<IFood>();
        if (!foods.Contains(food))
        {
            foods.Add(food);
            other.transform.SetParent(this.transform.Find("Food"));
        }
    }

    void RemoveFoodFromFoodsList(Collider other)
    {
        IFood food = other.GetComponent<IFood>();
        if (foods.Contains(food))
        {
            foods.Remove(food);
            other.transform.SetParent(FallingFoodsTrans);
        }
    }
}
