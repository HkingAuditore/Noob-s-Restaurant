using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFFire : Tool {

    private bool isFiring;
    private Transform fireAnchor;
    private GameObject firePrefab;
    private ParticleSystemManager particleSystemManager;

    private void Awake()
    {
        particleSystemManager = GameManager.Instance.particleSystemManager;
        firePrefab = Resources.Load<GameObject>("Prefabs/CampFire");
        fireAnchor = this.transform.Find("FireAnchor");
    }

    public override void DoCtrl()
    {
        base.DoCtrl();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isFiring)
            {
                particleSystemManager.AddFXPrefab(firePrefab, fireAnchor);
                isFiring = true;
            }
            else
            {
                particleSystemManager.StopFXAndRemove(fireAnchor.GetChild(0).gameObject);
                isFiring = false;
            }
        }
    }
}
