using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum MatterState
{
    Solid,
    Liquid,
    Gas
}

public class Ingredient : MonoBehaviour
{
    public FoodName FoodName { get; protected set; }

    protected float temperature;
    protected float heatTime;
    protected MatterState state;

    protected GameObject[] prefabs = new GameObject[3];
    public GameObject solidPrefab;
    public GameObject liquidPrefab;
    public GameObject gasPrefab;


    protected virtual void Start()
    {
        prefabs[(int)MatterState.Solid] = solidPrefab;
        prefabs[(int)MatterState.Liquid] = liquidPrefab;
        prefabs[(int)MatterState.Gas] = gasPrefab;
    }
}