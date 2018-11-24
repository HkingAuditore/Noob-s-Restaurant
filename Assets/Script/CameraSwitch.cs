using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tools
{
    None,
    Chopper,
    Sink,
    Pan,
    Pot
}

public class CameraSwitch : MonoBehaviour
{
    public GameObject thisCamera;
    //public GameObject mainCamera;
    public GameObject player;
    //bool IsChanged = false;
    //bool Isinrange = false;
    public Tools tool = Tools.None;
    public GameObject toolGo;
    //private bool isUsing = false;

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
            BeginToolCtrl();
            player.GetComponent<PlayerMove>().enabled = false;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            thisCamera.SetActive(false);
            StopToolCtrl();
            player.GetComponent<PlayerMove>().enabled = true;
        }
    }

    void OnTriggerExit()
    {
        //Isinrange = false;
    }

    void Update()
    {

    }

    private void BeginToolCtrl()
    {
        switch (tool)
        {
            case Tools.None:
                break;
            case Tools.Chopper:
                if (toolGo != null)
                    toolGo.GetComponent<ChopperCtrl>().isCtrlling = true;
                break;
            case Tools.Sink:
                break;
            case Tools.Pan:
                break;
            case Tools.Pot:
                break;
            default:
                break;
        }
    }

    private void StopToolCtrl()
    {
        switch (tool)
        {
            case Tools.None:
                break;
            case Tools.Chopper:
                if (toolGo != null)
                    toolGo.GetComponent<ChopperCtrl>().isCtrlling = false;
                break;
            case Tools.Sink:
                break;
            case Tools.Pan:
                break;
            case Tools.Pot:
                break;
            default:
                break;
        }
    }
}
