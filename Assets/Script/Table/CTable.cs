using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CTable : Table {

    [SerializeField]
    GameObject cCamera;

    protected override void GetCamera()
    {
        cameraGO = cCamera;
    }
}
