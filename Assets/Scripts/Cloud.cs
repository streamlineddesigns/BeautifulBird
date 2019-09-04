using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cloud : MonoBehaviour {
    protected Renderer rend;

    void Start() {
        rend = GetComponent<Renderer>();
    }
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            rend.enabled = false;
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            rend.enabled = true;
        }
    }
}