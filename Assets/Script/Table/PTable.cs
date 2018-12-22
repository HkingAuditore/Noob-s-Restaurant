using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PTable : Table {

    [SerializeField]
    GameObject pCamera;
    [SerializeField]
    GameObject tool;

    protected override void Awake()
    {
        base.Awake();
        thisCamera = pCamera;
        toolGo = tool;
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
