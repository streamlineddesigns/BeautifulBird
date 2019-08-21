using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {
    public static GameController Singleton; 
    public CharacterController CharacterController;
    public GameObject Bird;
    public bool flightControlIsOn;
    public bool groundControlIsOn;
    protected float leftHandTrigger;
    protected float rightHandTrigger;
    protected bool leftGripPressed;
    protected bool rightGripPressed;

    void Awake()
    {
        Singleton = this;
    }

    void Start() 
    {
        
    }
    void Update()
    {
        //MissionControlCheck();
    }

    protected void MissionControlCheck()
    {
        //If the player is touching the ground
        if (CharacterController.isGrounded) {
            //turn off flight control
            flightControlIsOn = false;
            //turn on ground control
            groundControlIsOn = true;
            //hide the bird
            //Bird.SetActive(false);

        //If the player is in the air
        } else if (!CharacterController.isGrounded ) {
            //turn on flight control
            flightControlIsOn = true;
            //turn off ground control
            groundControlIsOn = false;
            //show the bird
           // Bird.SetActive(true);
        }
    }
}