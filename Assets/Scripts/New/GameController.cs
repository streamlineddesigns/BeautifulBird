using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {
    public static GameController Singleton;
    public GameObject StartPanel;
    public GameObject LaserPointer;
    public GameObject OneSecondLeft;
    public GameObject TwoSecondLeft;
    public GameObject ThreeSecondLeft;
    public GameObject FlyingScripts;
    public GameObject WalkingScripts;
    public GameObject CountDownPanel;
    public GameObject Target;
    public CharacterController playerController;
    public GameObject flyingCamera;
    public GameObject walkingCamera;
    public int controllerSwitch;//0-flying;1-walkie;
    public GroundMovementModel GroundMovementModel;

    void Awake()
    {
        Singleton = this;
    }

    void Start() 
    {
        
    }

    void Update()
    {
        SwitchControllers();
    }
    public void StartGame() {
        LaserPointer.SetActive(false);
        StartPanel.SetActive(false);
        StartCoroutine(CountDown());
    }
    
    IEnumerator CountDown()
    {
        CountDownPanel.SetActive(true);
        ThreeSecondLeft.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        ThreeSecondLeft.SetActive(false);
        TwoSecondLeft.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        TwoSecondLeft.SetActive(false);
        OneSecondLeft.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        OneSecondLeft.SetActive(false);
        CountDownPanel.SetActive(false);
        WalkingScripts.SetActive(true);
        FlyingScripts.SetActive(true);
    }

    public void SwitchControllers()
    {
        if (Feet.Singleton.grounded) {
            controllerSwitch = 1;
            flyingCamera.SetActive(false);
            walkingCamera.SetActive(true);
            Target.SetActive(false);
            
        }
        
        /*else if (! Feet.Singleton.grounded) {
            //If they aren't jumping or falling
            if (! GroundMovementModel.bJumping && ! GroundMovementModel.bIsFalling) {
                //controllerSwitch = 0;
                //flyingCamera.SetActive(true);
                //walkingCamera.SetActive(false);
                //Target.SetActive(true);
            }
        }*/
    }

    public void Fly()
    {
        controllerSwitch = 0;
        flyingCamera.SetActive(true);
        walkingCamera.SetActive(false);
        Target.SetActive(true);
    }
}