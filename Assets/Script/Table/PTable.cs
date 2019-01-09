using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PTable : Table {

    [SerializeField]
    GameObject pCamera;

    protected override void GetCamera()
    {
        cameraGO = pCamera;
    }
}
