using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PTable : Table {

    [SerializeField]
    GameObject pCamera;
    [SerializeField]
    GameObject toolSet;

    protected override void GetCamera()
    {
        cameraGO = pCamera;
    }

    protected override void GetTool()
    {
        toolGo = toolSet;
    }
}
