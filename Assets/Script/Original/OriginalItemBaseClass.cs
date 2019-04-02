using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 食材基类
/// </summary>
namespace Original
{
    [System.Serializable]
    public class OriginalItemBaseClass : MonoBehaviour
    {
        [SerializeField]
        public OriginalsDataScriptTable data;
        
        [SerializeField]
        private int iD;
        public int ID
        {
            get
            {
                return iD;
            }
        }

        //加热指数
        public float heating;
        //融合/入味
        public float fuse;
        //标签
        public MainOriginalTag MainTag;
        public ViceOriginalTag viceTag;

        internal void Init() { Init(data); }

        internal void Init(OriginalsDataScriptTable data)
        {
            if (data == null) this.data = data;
        }

        /// <summary>
        /// 用于新生成赋值ID
        /// </summary>
        internal void Init(OriginalsDataScriptTable data, int _ID)
        {
            iD = _ID;
            if (data == null) this.data = data;
        }
        
        /// <summary>
        /// 存档
        /// </summary>
        /// <returns></returns>
        internal string SaveData() { return ""; }
    }
}