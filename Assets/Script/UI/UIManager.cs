using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UIManager {

    string uiObjectPath = Application.dataPath + "Prefab/UI";

    Canvas canvas;
    Dictionary<string,GameObject> currentSceneUI;

    void Init()
    {
        if (Directory.Exists(uiObjectPath))
        {

        }

    }

    void GetAllUI()
    {

    }
}
