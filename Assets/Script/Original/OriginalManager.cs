using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameConfig;
using UnityEditor;
using System;

/// <summary>
/// 食材管理
/// </summary>
namespace Original
{
    [Flags]
    public enum MainType { 主料, 配料 };
    [Flags]
    public enum OriginalType { Tomato = 0 , Potato  = 1, Egg = 2, Beef = 3, AAA = 4, BBB = 5};
    [Flags]
    public enum Characteristic { 蔬菜, 水果, 肉类, 豆制品, 爽脆, 绵软, 油腻, 浓郁香味, 清甜, 粘稠, 营养, 鲜嫩 ,特殊香味, 麻辣 };
    [Flags]
    public enum MainOriginalTag { 初态, 腌制过的, 丁状的, 块状的, 条状的, 打过的 }
    [Flags]
    public enum ViceOriginalTag { 液态, 固态 }

    public class OriginalManager : MonoSingleton<OriginalManager>
    {
        //食材属性
        private Dictionary<OriginalType, OriginalsDataScriptTable> datas = new Dictionary<OriginalType, OriginalsDataScriptTable>();
        public Dictionary<OriginalType, OriginalsDataScriptTable> Datas
        {
            get
            {
                return datas;
            }
        }

        public bool initDone = false;

        private void Start()
        {
            Init();
            GetComponent<OriginalLibrary>().Test();//test

        }

        public void Init()
        {
            InitOriginalsDataScriptTable();



            //---
            initDone = true;
        }

        /// <summary>
        /// 食材属性加载初始化
        /// </summary>
        public void InitOriginalsDataScriptTable()
        {
            var paths = Config.Instance.PathConfig;
            for (int i = 0; i < paths.pathConfigs.Count; i++)
            {
                var temp = Config.RelativeConfigPath + paths.pathConfigs[i].path;
                OriginalsDataScriptTable table = Resources.Load<OriginalsDataScriptTable>(temp);
                datas.Add((OriginalType)paths.pathConfigs[i].type, table);
            }
        }
    }
}