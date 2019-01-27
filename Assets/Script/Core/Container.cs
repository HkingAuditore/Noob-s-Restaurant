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
    protected float dropOffset = 0;
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

    public void AddToContents(Ingredient ingredient)
    {
        Contents.Add(ingredient);
        Vector3 v = (Vector3)UnityEngine.Random.insideUnitCircle * 0.5f * dropOffset;
        ingredient.transform.position = DropFoodPos + new Vector3(v.x, 0, v.y);
        ingredient.transform.SetParent(transform);
    }

    public void AddRange(List<Ingredient> ingredients, Vector3 posOffset)
    {
        //Transform parent = transform.Find("Contents");
        Array.ForEach(ingredients.ToArray(), (ingredient) =>
        {
            //Debug.DrawLine(ingredient.transform.position, ingredient.transform.position + posOffset, Color.red, 10000);
            ingredient.transform.position += posOffset;
            //ingredient.transform.position = DropFoodPos;
            ingredient.transform.SetParent(transform);
        });
        Contents.AddRange(ingredients);
        ingredients.Clear();
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

        container.AddToContents(ingredient);
        Contents.Remove(ingredient);
        return ingredient;
    }
    public Ingredient TakeTheOneTo(IContainable<Ingredient> container)
    {
        Contents.Sort();
        Ingredient ingredient = Contents.Last();
        container.AddToContents(ingredient);
        Contents.Remove(ingredient);
        return ingredient;
    }

    public virtual List<Ingredient> TakeOutAllTo(IContainable<Ingredient> container)
    {
        List<Ingredient> outList = new List<Ingredient>(Contents);
        //Debug.Log("utensil:" + container.DropFoodPos + "\tbowl:" + DropFoodPos + "\t" + ingredients.Count + "\t" + outList.Count + "\t" + (ingredients == outList) + "\t" + (ingredients.Equals(outList)));
        container.AddRange(outList, container.DropFoodPos - DropFoodPos);
        Contents.Clear();
        return outList;
    }

    public void Sort()
    {
        Contents.Sort((i1, i2) => i1.transform.localPosition.y.CompareTo(i2.transform.localPosition.y));
    }
}