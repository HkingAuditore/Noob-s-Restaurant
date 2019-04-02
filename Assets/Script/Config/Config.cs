using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace GameConfig
{
    //配置文件
    public class Config : MonoSingleton<Config>
    {
        private static readonly string relativeConfigPath = "Bin";
        public static string RelativeConfigPath
        {
            get
            {
#if UNITY_EDITOR
                return relativeConfigPath; 
#else
                return relativeConfigPath;
#endif
            }
        }

        //Relative
        private PathConfigScriptTable pathConfig = null;
        public PathConfigScriptTable PathConfig
        {
            get
            {
                if (!Initialized || pathConfig == null) Init();
                return pathConfig;
            }
        }

        private static string ConfigFilePathConfig = "/GameConfig/PathConfigScriptTable";

        private bool Initialized = false;

        public void Init()
        {
            string path = RelativeConfigPath + ConfigFilePathConfig;
            try
            {
                pathConfig = Resources.Load<PathConfigScriptTable>(path);
                Initialized = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("配置文件初始化失败 ：" + e.Message);
                Initialized = false;
            }
        }
    }
}