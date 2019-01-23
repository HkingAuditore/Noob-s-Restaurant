using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 器皿,简单容器
/// </summary>
public abstract class Ware : Container, IUsable
{
    private Transform FallingFoodsTrans;

    public override Vector3 DropFoodPos
    {
        get { return transform.position; }
        set { Debug.LogWarning("Setting DropFoodPos for Ware means nothing"); }
    }

    protected void Awake()
    {
        FallingFoodsTrans = GameObject.Find("Environment/Inside/FallingFoods").transform;
    }
    protected override void Start()
    {
        base.Start();
        InitIngredientsList();
    }
    protected void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ingredient>() != null)
            AddFoodIntoFoodsList(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Ingredient>() != null)
        {
            RemoveFoodFromFoodsList(other);
        }
    }

    /// <summary>
    /// 不同的容器重写不同的初始化子内容方法
    /// </summary>
    protected virtual void InitIngredientsList()
    {
    }

    void AddFoodIntoFoodsList(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (!Contents.Contains(ingredient))
        {
            Contents.Add(ingredient);
            other.transform.SetParent(transform);
        }
    }

    void RemoveFoodFromFoodsList(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (Contents.Contains(ingredient))
        {
            Contents.Remove(ingredient);
            other.transform.SetParent(FallingFoodsTrans);
        }
    }

    //IUsable Implement
    public bool isCtrlling = false;
    public bool IsCtrlling
    {
        get
        {
            return isCtrlling;
        }

        set
        {
            isCtrlling = value;
        }
    }

    public void BeginCtrl()
    {
        OnBeginCtrl();
        isCtrlling = true;
    }
    public void StopCtrl()
    {
        isCtrlling = false;
        OnStopCtrl();
    }

    public void DoCtrl()
    {
        throw new NotImplementedException();
    }

    public virtual void OnBeginCtrl()
    {
    }

    public virtual void OnStopCtrl()
    {
    }

}