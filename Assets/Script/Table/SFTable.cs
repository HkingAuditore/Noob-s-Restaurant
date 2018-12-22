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
        thisCamera = sfCamera;
        toolGo = toolSet;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }
}
