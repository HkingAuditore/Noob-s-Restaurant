using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float ver;
    public float hor;
    public bool isEDown;
    public bool isQDown;
    public bool isFDown;

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
