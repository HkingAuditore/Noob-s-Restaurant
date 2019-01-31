using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

//Json读写 -----------------------------------------
public class JsonHelper
{

    //读取json数据，根据id查找（表格里的第一行），返回id和类对象互相对应的字典。    
    public static T ReadJson<T>(string fileName)
    {
        TextAsset ta = Resources.Load(fileName) as TextAsset;
        if (ta.text == null)
        {
            Debug.Log("根据路径未找到对应表格数据");
        };
        T obj = JsonMapper.ToObject<T>(ta.text);
        return obj;
    }

}