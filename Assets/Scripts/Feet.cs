using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Feet : MonoBehaviour {
    public bool grounded;
    public static Feet Singleton;
    void Awake()
    {
        Singleton = this;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag != "Item" && other.tag != "Cloud" && other.tag != "MainCamera") {
            if (grounded != true) {
                grounded = true;
            }
        }
    }
    void OnTriggerStay(Collider other) {
        if (other.tag != "Item" && other.tag != "Cloud" && other.tag != "MainCamera") {
            if (grounded != true) {
                grounded = true;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag != "Item" && other.tag != "Cloud" && other.tag != "MainCamera") {
            if (grounded != false) {
                grounded = false;
            }
        }
    }
}