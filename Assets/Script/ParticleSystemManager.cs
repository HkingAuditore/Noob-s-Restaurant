using DigitalRuby.PyroParticles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager
{

    public void Init()
    {
    }

    public GameObject AddFXPrefab(GameObject prefab, Transform parent)
    {
        GameObject go = GameObject.Instantiate(prefab, parent);
        return go;
    }

    public void StopFXAndRemove(GameObject go)
    {
        go.GetComponent<FireConstantBaseScript>().Stop();
    }
}
