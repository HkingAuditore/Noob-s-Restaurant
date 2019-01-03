using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 容器，特指用于盛装食材的容器
/// </summary>
public abstract class Container : MonoBehaviour, IContainer<Ingredient>
{
    private Vector3 cenPos;
    protected float offset = 3;

    protected virtual void Start()
    {
        cenPos = transform.position;
    }

    protected List<Ingredient> ingredients = new List<Ingredient>();
    public List<Ingredient> Contents
    {
        get
        {
            return ingredients;
        }
    }

    public void Add(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
        ingredient.transform.position = cenPos + Vector3.up * 5 + (Vector3)Random.insideUnitCircle * offset;
    }

    public void AddRange(List<Ingredient> ingredients)
    {
        ingredients.AddRange(ingredients);
    }

    public Ingredient TakeOneTo(Ingredient ingredient, IContainer<Ingredient> container)
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
    public Ingredient TakeTheOneTo(IContainer<Ingredient> container)
    {
        Contents.Sort();
        Ingredient ingredient = Contents.Last();
        container.Add(ingredient);
        Contents.Remove(ingredient);
        return ingredient;
    }

    public List<Ingredient> TakeOutAllTo(IContainer<Ingredient> container)
    {
        List<Ingredient> outList = new List<Ingredient>(ingredients);
        container.AddRange(outList);
        ingredients.Clear();
        return outList;
    }

    public void Sort()
    {
        ingredients.Sort((i1, i2) => i1.transform.position.y.CompareTo(i2.transform.position.y));
    }

}