using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SFTable : Table {

    [SerializeField]
    GameObject sfCamera;

    protected override void GetCamera()
    {
        cameraGO = sfCamera;
    }

}
