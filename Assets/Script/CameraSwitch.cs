using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject thisCamera;
    //public GameObject mainCamera;
    public GameObject player;
    //bool IsChanged = false;
    //bool Isinrange = false;

    private void Start()
    {
        //IsChanged = false;
    }

    void OnTriggerEnter()
    {
        //Isinrange = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (Input.GetKey(KeyCode.E))
        {
            thisCamera.SetActive(true);
            player.GetComponent<PlayerMove>().enabled = false;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            thisCamera.SetActive(false);
            player.GetComponent<PlayerMove>().enabled = true;
        }
    }

    void OnTriggerExit()
    {
        //Isinrange = false;
    }

    //void Update()
    //{
    //    if (Input.GetKey(KeyCode.E) && (!IsChanged) && (Isinrange))
    //    {
    //        Debug.Log("0");
    //        //Camera.GetComponent<Camera>().enabled = true;
    //        //Maincamera.GetComponent<Camera>().enabled = false;
    //        thisCamera.SetActive(true);
    //        player.GetComponent<PlayerMove>().enabled = false;
    //        IsChanged = true;
    //        Debug.Log("1");
    //    }

    //    if (Input.GetKey(KeyCode.Q) && (IsChanged) && (Isinrange))
    //    {
    //        //Maincamera.GetComponent<Camera>().enabled = true;
    //        //Camera.GetComponent<Camera>().enabled = false;
    //        thisCamera.SetActive(false);
    //        player.GetComponent<PlayerMove>().enabled = true;
    //        IsChanged = false;
    //        Debug.Log("2");
    //    }
    //    // Debug.Log("in");
    //}
}
