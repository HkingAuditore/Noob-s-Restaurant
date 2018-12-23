using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CTable : Table {

    [SerializeField]
    GameObject cCamera;
    [SerializeField]
    GameObject toolSet; 

    protected override void Awake()
    {
        base.Awake();
        thisCameraGO = cCamera;
        toolGo = toolSet;
    }

    protected override void Start()
    {
        base.Start();
    }
}
