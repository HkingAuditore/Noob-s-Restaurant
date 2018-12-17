using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotCtrl : ToolCtrl
{
    public GameObject fire;

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
            if (fire.activeSelf)
            {
                //fire.GetComponent<FireConstantBaseScript>().Stop();
                fire.SetActive(false);
            }
            else
            {
                fire.SetActive(true);
                Array.ForEach(fire.GetComponentsInChildren<ParticleSystem>(), (ps) => ps.Play());
            }
        }
    }
}
