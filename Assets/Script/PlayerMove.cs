using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    private Rigidbody player_rigbody;
    public float rotate_speed;
    public float speed;

    void Move(){
        if (Input.GetKey(KeyCode.W))
        {
            player_rigbody.velocity = transform.forward * speed;
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.Rotate(0, -1 * rotate_speed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.Rotate(0, 1 * rotate_speed * Time.deltaTime, 0);
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            player_rigbody.velocity = -1*transform.forward * speed;
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.Rotate(0, 1 * rotate_speed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.Rotate(0, -1 * rotate_speed * Time.deltaTime, 0);
            }
        }
        
    }

    void Start(){
        player_rigbody = this.GetComponent<Rigidbody>();
    }


    void Update () {
        Move();
    }
}
