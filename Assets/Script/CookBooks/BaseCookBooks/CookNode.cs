using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Original;


namespace CookBooks
{
    public class CookNode : MonoBehaviour
    {
    }

    public class AssembleCookNode : CookNode
    {
        public CookNode[] InNodes = new CookNode[2];
    }

    public class ProcessCookNode : CookNode
    {
        public CookNode InNode;

        public ProcessTag ProcessTag;
    }

    public class OriginalCookNode : CookNode
    {
        public OriginalType OriginalType;
    }

    public class RootCookNode : CookNode
    {
        public CookNode InNode;
    }
}
