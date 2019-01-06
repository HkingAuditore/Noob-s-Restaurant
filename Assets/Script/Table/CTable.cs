using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CTable : Table {

    [SerializeField]
    GameObject cCamera;
    [SerializeField]
    GameObject toolSet; 

    protected override void GetCamera()
    {
        cameraGO = cCamera;
    }

    protected override void GetTool()
    {
        toolGo = toolSet;
    }
}
