﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Original
{
    public class OriginalCreator : MonoSingleton<OriginalCreator>
    {
        Dictionary<OriginalType, OriginalsDataScriptTable> Datas;

        /// <summary>
        /// 新创建食材
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public OriginalItemBaseClass CreateOriginal(OriginalType type)
        {
            if (!OriginalManager.Instance.initDone) {
                Debug.LogError("Configuration file not loaded yet!");
                return null;
            }
            OriginalItemBaseClass original = new OriginalItemBaseClass();
            original.Init(OriginalManager.Instance.Datas[type], OriginalIDCreator.GetID());
            original.data = OriginalManager.Instance.Datas[type];
            return original;
        }

        /// <summary>
        /// 从存档加载食材
        /// </summary>
        /// <returns></returns>
        public OriginalItemBaseClass LoadOriginal() { return null; }

        /// <summary>
        /// 食材销毁
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool DeleteOriginal(OriginalItemBaseClass item)
        {
            item = null;
            return true;
        }
    }

    public static class OriginalIDCreator
    {
        private static int index = -1;

        public static int GetID()
        {
            index++;
            return index;
        }

        public static void SetIndex(int maxID)
        {
            index = maxID;
        }
    }
}