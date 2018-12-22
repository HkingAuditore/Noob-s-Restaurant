using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour {

    protected GameObject player;
    protected PlayerCtrl playerCtrl;
    protected GameObject toolGo;
    protected GameObject[] foodSets;
    protected GameObject thisCamera;

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCtrl = player.GetComponent<PlayerCtrl>();
    }

    protected virtual void Start()
    {
        if (thisCamera != null)
            thisCamera.SetActive(false);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        SwitchCamera(other);
    }

    private void SwitchCamera(Collider other)
    {
        if (!other.CompareTag("Player") || thisCamera == null)
            return;

        if (Input.GetKey(KeyCode.E))
        {
            thisCamera.SetActive(true);
            playerCtrl.Hide();
            playerCtrl.isCanCtrl = false;
            if (toolGo != null && toolGo.GetComponent<ToolCtrl>() != null)
                toolGo.GetComponent<ToolCtrl>().BeginCtrl();
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            thisCamera.SetActive(false);
            playerCtrl.Show();
            playerCtrl.isCanCtrl = true;
            if (toolGo != null && toolGo.GetComponent<ToolCtrl>() != null)
                toolGo.GetComponent<ToolCtrl>().StopCtrl();
        }
    } 
}
