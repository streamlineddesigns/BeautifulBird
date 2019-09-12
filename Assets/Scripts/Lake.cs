using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Lake : MonoBehaviour {
    protected AudioSource AudioSource;

    void Start() {
        AudioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            AudioSource.Play();
        }
    }
}