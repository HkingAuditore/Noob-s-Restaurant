using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float ver;
    public float hor;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ver = Input.GetAxis("Vertical");
        hor = Input.GetAxis("Horizontal");
    }
}
