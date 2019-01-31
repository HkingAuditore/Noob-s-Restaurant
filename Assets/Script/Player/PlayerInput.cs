using System;
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

    public event Action<AttentionType, object> KeyDownEvent;

    // Use this for initialization
    void Start()
    {
        KeyDownEvent += GameManager.Instance.sequenceManager.StepTriggerHandler;
    }

    // Update is called once per frame
    void Update()
    {
        ver = Input.GetAxis("Vertical");
        hor = Input.GetAxis("Horizontal");

        if (Input.GetMouseButtonDown(1))
            if (KeyDownEvent != null)
                KeyDownEvent(AttentionType.KeyDown, KeyCode.Mouse1);
    }

    private void OnGUI()
    {
        Event e = Event.current;
        //Debug.Log(Event.current.type);
        //if (AnyKeyControl_Bool)
        {
            if (e.isKey)
            {
                if (Input.anyKeyDown)
                {
                    //清空按下帧数
                    //keyFrame = 0;
                    //Debug.Log("任意键被按下");
                    //Debug.Log("Detected key code 1: " + e.keyCode);
                    if (KeyDownEvent != null)
                        KeyDownEvent(AttentionType.KeyDown, e.keyCode);
                }

                //if (Input.anyKey)
                //{
                //    keyFrame++;
                //    timeDelay += Time.deltaTime;
                //    TestAnyKey = e.keyCode;
                //    Debug.Log("任意键被长按" + keyFrame + "帧");
                //    Debug.Log("Detected key code 2: " + TestAnyKey);
                //    Debug.Log("时间： " + timeDelay);
                //}
            }
        }
    }
}
