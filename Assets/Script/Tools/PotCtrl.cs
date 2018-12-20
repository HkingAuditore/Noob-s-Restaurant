using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotCtrl : ToolCtrl
{
    private ParticleSystemManager particleSystemManager;

    public Transform fireAnchor;
    private GameObject firePrefab;
    private bool isFiring;

    private void Start()
    {
        particleSystemManager = GameManager.Instance.particleSystemManager;
        firePrefab = Resources.Load<GameObject>("Prefabs/CampFire");
    }

    private void Update()
    {
        if (!isCtrlling)
            return;

        DoAction();
    }

    private void DoAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isFiring)
            {
                //fire.GetComponent<FireConstantBaseScript>().Stop();
                //Array.ForEach(fireAnchor.GetComponentsInChildren<ParticleSystem>(), (ps) => ps.Stop());
                //fireAnchor.SetActive(false);
                particleSystemManager.AddFXPrefab(firePrefab, fireAnchor);
                isFiring = true;
            }
            else
            {
                //fireAnchor.SetActive(true);
                //Array.ForEach(fireAnchor.GetComponentsInChildren<ParticleSystem>(), (ps) => ps.Play());
                particleSystemManager.StopFXAndRemove(fireAnchor.GetChild(0).gameObject);
                isFiring = false;
            }
        }
    }
}
