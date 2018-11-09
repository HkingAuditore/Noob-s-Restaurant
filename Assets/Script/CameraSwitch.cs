using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour {

    public  GameObject Camera;
    public  GameObject Maincamera;
    public GameObject Player;
    bool IsChanged=false;
    bool Isinrange = false;

    private void Start()
    {
        IsChanged = false;
    }

    void OnCollisionEnter()
    {
        Isinrange = true;
    }

    void OnCollisionExit()
    {
        Isinrange = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E)&&(!IsChanged)&&(Isinrange))  
        {
            Debug.Log("0");
            Camera.GetComponent<Camera>().enabled = true;
            Maincamera.GetComponent<Camera>().enabled = false;
            Player.GetComponent<PlayerMove>().enabled = false;
            IsChanged = true;
            Debug.Log("1");
        }

        if (Input.GetKey(KeyCode.Q) && (IsChanged)&&(Isinrange))
        {
            Maincamera.GetComponent<Camera>().enabled = true;
            Camera.GetComponent<Camera>().enabled = false;
            Player.GetComponent<PlayerMove>().enabled = true;
            IsChanged = false;
            Debug.Log("2");
        }
       // Debug.Log("in");
    }
}
