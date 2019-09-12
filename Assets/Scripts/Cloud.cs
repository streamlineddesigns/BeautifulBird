using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cloud : MonoBehaviour {
    protected Renderer rend;
    protected AudioSource AudioSource;

    void Start() {
        rend = GetComponent<Renderer>();
        AudioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            rend.enabled = false;
            AudioSource.Play();
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            rend.enabled = true;
        }
    }
}