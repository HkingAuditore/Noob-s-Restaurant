using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 容器，特指用于盛装食材的容器
/// </summary>
public abstract class Container : MonoBehaviour, IContainable<Ingredient>
{

    public virtual Vector3 DropFoodPos { get; set; }

    protected List<Ingredient> ingredients = new List<Ingredient>();
    public List<Ingredient> Contents
    {
        get
        {
            return ingredients;
        }
    }

    protected virtual void Start() { }

    public void Add(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
        ingredient.transform.localPosition = DropFoodPos + ingredient.transform.up * 1 + (Vector3)UnityEngine.Random.insideUnitCircle * 0.1f;
        ingredient.transform.SetParent(transform);
    }

    public void AddRange(List<Ingredient> ingredients, Vector3 posOffset)
    {
        Debug.Log(posOffset);
        ingredients.AddRange(ingredients);
        Array.ForEach(ingredients.ToArray(), (ingredient) =>
        {
            //ingredient.transform.position += posOffset;
            ingredient.transform.position = DropFoodPos;
            ingredient.transform.SetParent(transform);
        });
    }

    public Ingredient TakeOneTo(Ingredient ingredient, IContainable<Ingredient> container)
    {
        //int index = Random.Range(0, container.Ingredients.Count);
        //int index = container.Ingredients.Where(//((ingredient) => { return ingredient.transform.position.y; });

        if (!Contents.Contains(ingredient))
        {
            Debug.LogError("the given ingredient does not exist in this container");
            return null;
        }

        container.Add(ingredient);
        Contents.Remove(ingredient);
        return ingredient;
    }
    public Ingredient TakeTheOneTo(IContainable<Ingredient> container)
    {
        Contents.Sort();
        Ingredient ingredient = Contents.Last();
        container.Add(ingredient);
        Contents.Remove(ingredient);
        return ingredient;
    }

    public virtual List<Ingredient> TakeOutAllTo(IContainable<Ingredient> container)
    {
        List<Ingredient> outList = new List<Ingredient>(ingredients);
        Debug.Log("utensil:" + container.DropFoodPos + "\tbowl:" + DropFoodPos);
        container.AddRange(outList, container.DropFoodPos - DropFoodPos);
        ingredients.Clear();
        return outList;
    }

    public void Sort()
    {
        ingredients.Sort((i1, i2) => i1.transform.localPosition.y.CompareTo(i2.transform.localPosition.y));
    }

}