using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BTable : Table {

    [SerializeField]
    GameObject bCamera;
    [SerializeField]
    GameObject toolSet;

    protected override void Awake()
    {
        base.Awake();
        thisCameraGO = bCamera;
        toolGo = toolSet;
    }

    protected override void Start()
    {
        base.Start();
    }
}
