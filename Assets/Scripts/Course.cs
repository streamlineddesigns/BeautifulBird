using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Course : MonoBehaviour {
    public int position;
    public int end;
    public AudioSource Collected;

    void start() {
        position = 0;
        end = transform.childCount;
    }
    public void Next() {
        Collected.Play();
        transform.GetChild(position).gameObject.SetActive(false);
        position++;
        //if we're not at the end of the course
        if (position < end) {
            transform.GetChild(position).gameObject.SetActive(true);
            // if we're at the end of the course
        } else if (position == end){
            transform.GetChild(position).gameObject.SetActive(true);
            //StartCoroutine(ResetCourse());
        }
    }

    IEnumerator ResetCourse()
    {
        yield return new WaitForSeconds(7.0f);
        transform.GetChild(position).gameObject.SetActive(false);
        position = 0;
        transform.GetChild(position).gameObject.SetActive(true);
    }
}