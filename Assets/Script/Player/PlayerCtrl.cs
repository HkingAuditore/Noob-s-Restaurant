using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private Rigidbody rig;
    private PlayerInput input;
    private Animator animator;
    private GameObject model;
    public GameObject heldFoodSet;
    public Transform holdFoodMarkTrans;
    private MatchTargetWeightMask matchTargetWeightMask;
    public bool isCanCtrl = true;
    public bool isHoldFoodSet = false;

    public float rotate_speed;
    public float speed;

    private void Awake()
    {
        rig = this.GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        model = transform.Find("Model").gameObject;
        holdFoodMarkTrans = transform.Find("HoldFoodMark").transform;
        matchTargetWeightMask = new MatchTargetWeightMask(new Vector3(1, 1, 1), 0);
    }

    void FixedUpdate()
    {
        Move();
        SwitchHoldBolw();
    }

    void SwitchHoldBolw()
    {
        if (isHoldFoodSet)
        {
            animator.SetFloat("isHoldBowl", 1);
        }
        else
        {
            animator.SetFloat("isHoldBowl", 0);
        }
    }

    void Move()
    {
        if (!isCanCtrl)
            return;

        if (Mathf.Abs(input.hor) > 1e-6 || Mathf.Abs(input.ver) > 1e-6)
        {
            transform.position += new Vector3(input.hor * speed * Time.deltaTime, 0, input.ver * speed * Time.deltaTime);
            transform.LookAt(transform.position + Vector3.right * input.hor + Vector3.forward * input.ver);

            animator.SetFloat("walk", Mathf.Sqrt(input.hor * input.hor + input.ver * input.ver));
        }
    }

    public void Hide()
    {
        animator.SetFloat("walk", 0);
        model.SetActive(false);
    }

    public void Show()
    {
        model.SetActive(true);
    }

}
