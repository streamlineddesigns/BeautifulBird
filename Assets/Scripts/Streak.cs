using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Streak : MonoBehaviour {
    public float initialTimer;
    public float timeLeft;
    public int stage;
    protected TrailRenderer trail;

    void Start()
    {
        initialTimer = Random.Range(0.0f, 3.0f);
        timeLeft = initialTimer;
        stage = 0;
        trail = GetComponent<TrailRenderer>();
    }
    void Update()
    {
        //timer ran out
        if (timeLeft < 0) {
            //new time
            timeLeft = Random.Range(0.0f, 3.0f);

            if (stage == 0) {
                stage++;
                trail.enabled = true;
            } else if (stage == 1) {
                stage = 0;
                trail.enabled = false;
            }

        //timer hasn't run out
        } else {
            timeLeft -= Time.deltaTime;
        }
        
    }


}