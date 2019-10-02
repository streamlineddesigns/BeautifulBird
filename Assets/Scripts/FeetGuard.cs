using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IF feet guard is hitting something, the player needs to be moving up
public class FeetGuard : MonoBehaviour {
    public bool grounded;
    public static FeetGuard Singleton;
    void Awake()
    {
        Singleton = this;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag != "Item" && other.tag != "Cloud" && other.tag != "MainCamera" && other.tag != "Bird") {
            if (grounded != true) {
                grounded = true;
            }
        }
    }
    void OnTriggerStay(Collider other) {
        if (other.tag != "Item" && other.tag != "Cloud" && other.tag != "MainCamera" && other.tag != "Bird") {
            if (grounded != true) {
                grounded = true;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag != "Item" && other.tag != "Cloud" && other.tag != "MainCamera" && other.tag != "Bird") {
            if (grounded != false) {
                grounded = false;
            }
        }
    }
}