using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stream : MonoBehaviour
{
    private LineRenderer _lineRenderer = null;
    private ParticleSystem _spalashEffect = null;
    private Coroutine _pourRoutine = null;
    private Vector3 _targetPosition = Vector3.zero;
    
    public float speed;
    public float maxLength;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _spalashEffect = GetComponentInChildren<ParticleSystem>();
        Debug.Log(_spalashEffect);
        
    }

    private void Start()
    {
//        Move2Position(0,new Vector3(0,0,0));
        Move2Position(0,transform.position);
        Move2Position(1,transform.position);
    }

    public void Begin()
    {
        StartCoroutine(UpdateParticle());
        _pourRoutine = StartCoroutine(BeginPour());
    }

    private IEnumerator BeginPour()
    {
        while (gameObject.activeSelf)
        {
            Debug.Log("in!");
            _targetPosition = FindEndPoint();
            Move2Position(0,transform.position);
            Animate2Position(1,_targetPosition);

            yield return null;
        }
    }

    public void End()
    {
        StopCoroutine(_pourRoutine);
        _pourRoutine = StartCoroutine(EndPour());
    }

    private IEnumerator EndPour()
    {
        while (!HasReachedPosition(0,_targetPosition))
        {
            Animate2Position(0,_targetPosition);
            Animate2Position(1,_targetPosition);
            yield return null;
        }
        Destroy(gameObject);
    }

    private Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position,Vector3.down);
        Physics.Raycast(ray, out hit, maxLength);
        Vector3 endPoint = hit.collider ? new Vector3(hit.point.x,hit.point.y+0.01f,hit.point.z) : ray.GetPoint(maxLength);
        return endPoint;
    }

    private void Move2Position(int index,Vector3 targetPosition)
    {
        _lineRenderer.SetPosition(index,targetPosition);
    }

    private void Animate2Position(int index,Vector3 targetPosition)
    {
        Vector3 currentPoint = _lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition,Time.deltaTime * speed);
        
        _lineRenderer.SetPosition(index,newPosition);
    }

    private bool HasReachedPosition(int index, Vector3 target)
    {
        return _lineRenderer.GetPosition(index) == target;
    }

    private IEnumerator UpdateParticle()
    {
        while (gameObject.activeSelf)
        {
            _spalashEffect.gameObject.transform.position = _targetPosition;
            bool isHitting = HasReachedPosition(1, _targetPosition);
            _spalashEffect.gameObject.transform.eulerAngles = new Vector3(0,0,0);
            _spalashEffect.gameObject.SetActive(isHitting);
            yield return null;
        }
    }
}
