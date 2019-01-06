using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BTable : Table {

    [SerializeField]
    GameObject bCamera;
    [SerializeField]
    GameObject toolSet;

    protected override void GetCamera()
    {
        cameraGO = bCamera;
    }

    protected override void GetTool()
    {
        toolGo = toolSet;
    }

}
