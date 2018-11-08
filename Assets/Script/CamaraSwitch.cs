using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraSwitch : MonoBehaviour {

    public GameObject Camara;
    public GameObject Maincamara;
    //Transform Origin;
    bool IsChanged=false;


    
    void OnCollisionStay(Collision other)
    {
        if (Input.GetKey(KeyCode.E)&&(!IsChanged))  
        {
            Debug.Log("0");
            Maincamara.SetActive(false);
            Camara.SetActive(true);
            IsChanged = true;
            Debug.Log("1");
        }

        if (Input.GetKey(KeyCode.E) && (IsChanged))
        {
            Maincamara.SetActive(true);
            Camara.SetActive(false);
            IsChanged = false;
        }
        Debug.Log("in");
    }
}
