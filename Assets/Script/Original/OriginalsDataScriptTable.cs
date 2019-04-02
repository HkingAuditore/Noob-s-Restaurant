using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Original;

/// <summary>
/// 食材属性Config类
/// </summary>
[CreateAssetMenu(fileName = "OriginalsDataScriptTable", menuName = "Config/OriginalsDataScriptTable")][System.Serializable]
public class OriginalsDataScriptTable : ScriptableObject
{
    public MainType mainType;
    public OriginalType originalType;
    //特性标签
    public List<Characteristic> characteristic = new List<Characteristic>();
    //酸度
    public float acidity_A;
    public float acidity_B;
    //甜度
    public float sweetness_A;
    public float sweetness_B;
    //Spicy辣
    public float spicy_A;
    public float spicy_B;
    //Salinity咸
    public float salinity_A;
    public float salinity_B;
    //Freshness鲜
    public float Freshness_A;
    public float Freshness_B;
}
