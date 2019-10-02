using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cloud : MonoBehaviour {
    protected GameObject currentFog;
    protected MeshRenderer MR;
    void Start()
    {
        MR = GetComponent<MeshRenderer>();
        currentFog = transform.GetChild(0).gameObject;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bird"){
            if (MR.enabled) {
                //disable the clouds renderer
                MR.enabled = false;
                //show the fog
                currentFog.SetActive(true);
            } else {
                //enable the clouds renderer
                MR.enabled = true;
                //hide the fog
                currentFog.SetActive(false);
            }
        } 
    }
}