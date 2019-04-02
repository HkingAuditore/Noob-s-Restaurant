using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameConfig
{
    public class PathConfigScriptTable : ScriptableObject
    {
        public List<PathConfigClass> pathConfigs = new List<PathConfigClass>();
    }

    [System.Serializable]
    public class PathConfigClass
    {
        public int type;
        public string path;
    }
}
