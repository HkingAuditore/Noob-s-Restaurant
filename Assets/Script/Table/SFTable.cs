using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SFTable : Table {

    [SerializeField]
    GameObject sfCamera;
    [SerializeField]
    GameObject toolSet;

    protected override void Awake()
    {
        base.Awake();
        thisCameraGO = sfCamera;
        toolGo = toolSet;
    }

    protected override void Start()
    {
        base.Start();
    }
}
