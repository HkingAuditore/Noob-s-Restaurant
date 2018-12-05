using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody rig;
    private PlayerInput input;
    private Animator animator;
    public float rotate_speed;
    public float speed;



    void Move()
    {
        //if (Input.GetKey(KeyCode.W))
        //{
        //    player_rigbody.velocity = transform.forward * speed;
        //    if (Input.GetKey(KeyCode.A))
        //    {
        //        this.transform.Rotate(0, -1 * rotate_speed * Time.deltaTime, 0);
        //    }
        //    if (Input.GetKey(KeyCode.D))
        //    {
        //        this.transform.Rotate(0, 1 * rotate_speed * Time.deltaTime, 0);
        //    }
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    player_rigbody.velocity = -1*transform.forward * speed;
        //    if (Input.GetKey(KeyCode.A))
        //    {
        //        this.transform.Rotate(0, 1 * rotate_speed * Time.deltaTime, 0);
        //    }
        //    if (Input.GetKey(KeyCode.D))
        //    {
        //        this.transform.Rotate(0, -1 * rotate_speed * Time.deltaTime, 0);
        //    }
        //}
        if (Mathf.Abs(input.hor) > 1e-6 || Mathf.Abs(input.ver) > 1e-6)
        {
            transform.position += new Vector3(input.hor * speed * Time.deltaTime, 0, input.ver * speed * Time.deltaTime);
            transform.LookAt(transform.position + Vector3.right * input.hor + Vector3.forward * input.ver);

            animator.SetFloat("walk", Mathf.Sqrt(input.hor * input.hor + input.ver * input.ver));
        }
    }

    void Start()
    {
        rig = this.GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }


    void FixedUpdate()
    {
        Move();
    }
}
