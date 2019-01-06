using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SFTable : Table {

    [SerializeField]
    GameObject sfCamera;
    [SerializeField]
    GameObject toolSet;

    protected override void GetCamera()
    {
        cameraGO = sfCamera;
    }

    protected override void GetTool()
    {
        toolGo = toolSet;
    }

}
