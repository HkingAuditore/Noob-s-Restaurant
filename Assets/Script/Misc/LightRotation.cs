using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightRotation : MonoBehaviour {
    public float speed;
    public GameObject sun;

    void Update () {
        sun.transform.Rotate(Vector3.up * Time.deltaTime*speed, Space.World);

    }
}
