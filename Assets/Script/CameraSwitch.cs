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

    private PlayerCtrl playerCtrl;

    private void Start()
    {
        //IsChanged = false;
        playerCtrl = player.GetComponent<PlayerCtrl>();
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
        }

        if (Input.GetKey(KeyCode.Q))
        {
            thisCamera.SetActive(false);
            StopToolCtrl();
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
        playerCtrl.Hide();
        playerCtrl.isCanCtrl = false;

        if (toolGo.GetComponent<ToolCtrl>() != null)
            toolGo.GetComponent<ToolCtrl>().BeginCtrl();
        //switch (tool)
        //{
        //    case Tools.None:
        //        break;
        //    case Tools.Chopper:
        //        if (toolGo != null)
        //        {
        //            ChopperCtrl ctrl = toolGo.GetComponent<ChopperCtrl>();
        //            ctrl.isCtrlling = true;
        //            ctrl.SetOriPos();
        //        }
        //        break;
        //    case Tools.Sink:
        //        break;
        //    case Tools.Pan:
        //        if (toolGo != null)
        //        {
        //            TurnerCtrl ctrl = toolGo.GetComponent<TurnerCtrl>();
        //            ctrl.isCtrlling = true;
        //            ctrl.SetOriPos();
        //        }
        //        break;
        //    case Tools.Pot:
        //        break;
        //    default:
        //        break;
        //}
    }

    private void StopToolCtrl()
    {
        playerCtrl.Show();
        playerCtrl.isCanCtrl = true;

        if (toolGo.GetComponent<ToolCtrl>() != null)
            toolGo.GetComponent<ToolCtrl>().StopCtrl();
        //switch (tool)
        //{
        //    case Tools.None:
        //        break;
        //    case Tools.Chopper:
        //        if (toolGo != null)
        //            toolGo.GetComponent<ChopperCtrl>().isCtrlling = false;
        //        break;
        //    case Tools.Sink:
        //        break;
        //    case Tools.Pan:
        //        if (toolGo != null)
        //            toolGo.GetComponent<TurnerCtrl>().isCtrlling = false;
        //        break;
        //    case Tools.Pot:
        //        break;
        //    default:
        //        break;
        //}
    }
}
