using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public float pourThreshold = 0.45f;
    public Transform origin = null;
    public GameObject streamPrefab = null;

    private bool _isPouring = false;
    private Stream _currentStream = null;

    private void Update()
    {
        bool pourCheck = CalculatePourAngle() > pourThreshold;
        if (_isPouring != pourCheck)
        {
            _isPouring = pourCheck;
            if (_isPouring)
            {
                StartPour();
            }
            else
            {
                EndPour();
            }
        }
    }

    private void StartPour()
    {
        Debug.Log("Start");
        _currentStream = CreateStream();
        _currentStream.Begin();
    }

    private void EndPour()
    {
        Debug.Log("End");
        _currentStream.End();
        _currentStream = null;
    }

    private float CalculatePourAngle()
    {
        Debug.Log(Mathf.Abs(this.transform.rotation.x));
        return Mathf.Abs(this.transform.rotation.x) ;
    }

    private Stream CreateStream()
    {
        GameObject streamObj = Instantiate(streamPrefab, origin.position, Quaternion.identity, transform);
        streamObj.transform.rotation = new Quaternion(0,0,0,0);
        return streamObj.GetComponent<Stream>();
    }
}
