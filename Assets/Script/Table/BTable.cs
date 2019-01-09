using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BTable : Table {

    [SerializeField]
    GameObject bCamera;

    protected override void GetCamera()
    {
        cameraGO = bCamera;
    }
}
