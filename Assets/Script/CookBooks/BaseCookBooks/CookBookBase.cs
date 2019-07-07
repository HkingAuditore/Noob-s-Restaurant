using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookBooks
{
    public enum ProcessTag
    {
        腌制, 切丁, 切块, 切条, 打碎
    }

    public class CookBookBase : MonoBehaviour
    {
        public RootCookNode RootNode;
        public CookBoosInfo Info;
    }

    public class CookBoosInfo
    {
        public string CookBookName;
        public string Description;
    }
}