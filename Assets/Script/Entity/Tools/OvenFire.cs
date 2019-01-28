using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenFire : Tool
{

    private bool isFiring = false;
    private Transform fireAnchor;
    private GameObject firePrefab;
    private ParticleSystemManager particleSystemManager;

    protected override void Start()
    {
        base.Start();

        particleSystemManager = GameManager.Instance.particleSystemManager;
        firePrefab = Resources.Load<GameObject>("Prefabs/CampFire");
        fireAnchor = this.transform.Find("FireAnchor");
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        if (Input.GetKeyDown(KeyCode.F))
            SwitchFire();
    }

    private void SwitchFire()
    {
        if (!isFiring)
        {
            particleSystemManager.AddFXPrefab(firePrefab, fireAnchor);
            isFiring = true;

            Timer t = transform.parent.parent.GetComponent<Table>().HeatTimer;
            if (t != null)
            {
                t.Start();
            }
        }
        else
        {
            particleSystemManager.StopFXAndRemove(fireAnchor.GetChild(0).gameObject);
            isFiring = false;

            Timer t = transform.parent.parent.GetComponent<Table>().HeatTimer;
            if (t != null)
            {
                t.Stop();
            }
        }
    }
}
