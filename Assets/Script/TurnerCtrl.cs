using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnerCtrl : MonoBehaviour
{
    public bool isCtrlling = false;

    private Vector3 oriPos;
    private Rigidbody rb;
    public float offsetX = 0.8f;
    public float offsetZ = 0.8f;
    public float downY = 0.5f;
    private float targetDownY;

    private Vector3 lastMousePos;

    // Use this for initialization
    void Start()
    {
        oriPos = transform.localPosition;
        lastMousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        rb = GetComponent<Rigidbody>();
        targetDownY = oriPos.y - downY;
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

        this.transform.rotation = Quaternion.Euler(0f, 0, 0);
        rb.useGravity = false;
        rb.isKinematic = true;                                                                 //拿起后摆正

        Move();
        DoAction();
    }

    private void Move()
    {
        Vector3 delPos = (Input.mousePosition - lastMousePos) * 0.001f;
        //Vector3 pos = new Vector3();
        //pos.z = -delPos.x;
        //pos.y = delPos.y;
        //pos.x = 0;
        Vector3 targetPos = transform.localPosition + new Vector3(-delPos.y, 0, delPos.x);
        targetPos.x = Mathf.Clamp(targetPos.x, oriPos.x - offsetX, oriPos.x + offsetX);
        targetPos.z = Mathf.Clamp(targetPos.z, oriPos.z - offsetZ, oriPos.z + offsetZ);
        transform.localPosition = targetPos;

        lastMousePos = Input.mousePosition;
    }

    private void DoAction()
    {
        if (Input.GetMouseButton(0))
        {
            float y = Mathf.Lerp(transform.localPosition.y, targetDownY, 0.3f);
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }
        else
        {
            float y = Mathf.Lerp(transform.localPosition.y, oriPos.y, 0.3f);
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }
    }
}
