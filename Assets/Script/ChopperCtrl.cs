using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopperCtrl : MonoBehaviour
{
    public bool isCtrlling = false;

    private Vector3 oriPos;
    private Rigidbody rb;
    public float offsetY = 0.3f;
    public float offsetZ = 0.8f;

    private Vector3 lastMousePos;

    // Use this for initialization
    void Start()
    {
        oriPos = transform.localPosition;
        lastMousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCtrlling)
        {
            rb.useGravity = true;          //当玩家不使用时受重力控制
            rb.isKinematic = false;
            return;
        }

        this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        rb.useGravity = false;
        rb.isKinematic = true;//拿起后摆正


        Vector3 delPos = (Input.mousePosition - lastMousePos) * 0.001f;
        //Vector3 pos = new Vector3();
        //pos.z = -delPos.x;
        //pos.y = delPos.y;
        //pos.x = 0;
        Vector3 targetPos = oriPos + new Vector3(0, delPos.y, -delPos.x);
        targetPos.y = Mathf.Clamp(targetPos.y, oriPos.y - offsetY, oriPos.y + offsetY);
        targetPos.z = Mathf.Clamp(targetPos.z, oriPos.z - offsetZ, oriPos.z + offsetZ);
        transform.localPosition = targetPos;

        lastMousePos = Input.mousePosition;
    }

    public void SetOriPos()
    {
        transform.position = oriPos;
    }
}
