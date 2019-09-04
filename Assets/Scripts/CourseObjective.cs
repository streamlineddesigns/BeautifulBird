using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CourseObjective : MonoBehaviour {
    public bool used;
    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Bird") {
            if (! used) {
                used = true;
                transform.parent.GetComponent<Course>().Next();
            }
            
        }
    }
}