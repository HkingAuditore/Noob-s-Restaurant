using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopperCtrl : MonoBehaviour
{
    public bool isCtrlling = false;

    private Vector3 oriPos;
    public float offsetY = 0.3f;
    public float offsetZ = 0.8f;

    private Vector3 lastMousePos;

    // Use this for initialization
    void Start()
    {
        oriPos = transform.position;
        lastMousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCtrlling)
            return;

        Vector3 delPos = (Input.mousePosition - lastMousePos) * 0.001f;
        //Vector3 pos = new Vector3();
        //pos.z = -delPos.x;
        //pos.y = delPos.y;
        //pos.x = 0;
        Vector3 targetPos = transform.position + new Vector3(0, delPos.y, -delPos.x);
        targetPos.y = Mathf.Clamp(targetPos.y, oriPos.y - offsetY, oriPos.y + offsetY);
        targetPos.z = Mathf.Clamp(targetPos.z, oriPos.z - offsetZ, oriPos.z + offsetZ);
        transform.position = targetPos;

        lastMousePos = Input.mousePosition;
    }
}
