using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SMTable : Table
{
    [SerializeField]
    private GameObject smCamera;
    [SerializeField]
    GameObject tool;

    protected override void Awake()
    {
        base.Awake();
        thisCamera = smCamera;
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
