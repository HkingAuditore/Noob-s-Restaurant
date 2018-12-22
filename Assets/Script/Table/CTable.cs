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
        thisCamera = cCamera;
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
