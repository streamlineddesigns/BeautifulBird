using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlyingController : MonoBehaviour {
    protected FlyingModel FlyingModel;
    public GameObject RC;
    public GameObject LC;
    public GameObject Bird;
    public CharacterController BirdController;
    public float originalFlyingSpeed;
    public GameObject cameraContainer;
    public GameObject cameraTarget;

	void Awake () {
        FlyingModel = GetComponent<FlyingModel>();
	}

    void Start() {
        //thresholds
        FlyingModel.pitchThreshold = 1.0f;
        FlyingModel.rollThreshold = 1.0f;
        FlyingModel.wingFlapThreshold = 20.0f;

        //Speed
        FlyingModel.originalFlyingSpeed = 3.0f;
        FlyingModel.flyingSpeed = FlyingModel.originalFlyingSpeed;
        FlyingModel.maxSpeed = 8.0f;
        FlyingModel.minSpeed = -0.1f;
        FlyingModel.decelerationSpeedMultipler = -0.0075f;
        FlyingModel.accelerationSpeedMultipler = 0.02f;
    }

	void Update() {
        if (! GameController.Singleton.flightControlIsOn) {
            return;
        }

        //checks wing flapping
        FlapWingsCheck();

        if (FlyingModel.bLerpFlap) {
            FlapWingsAction();
        }

        if (BirdController.isGrounded) {
            return;
        }

        //camera follows bird
        followBird();

        //Get Input from the player
        InputChecks();
        
        //If the player is touching the ground
        if (BirdController.isGrounded) {

            if (FlyingModel.flyingSpeed != FlyingModel.originalFlyingSpeed) {
                FlyingModel.flyingSpeed = FlyingModel.originalFlyingSpeed;
            }

            //Walking Stuff?
            
        //If the player is in the air
        } else if (! BirdController.isGrounded){

            //Add Gravity to the bird
            ApplyGravity();

            //Out put orientation
            OutputOrientation();

            //Move Forward in air
            FlyForward();
        }
    }
    
    protected void followBird()
    {
        //Set Position
        Vector3 desiredPosition = cameraTarget.transform.position;
        cameraContainer.transform.position = Vector3.Lerp(cameraContainer.transform.position, desiredPosition, 1.0f);

        //Set Rotation
        Vector3 desiredRotation;
        desiredRotation = cameraContainer.transform.eulerAngles;
        //desiredRotation.x = Bird.transform.eulerAngles.x;
        desiredRotation.y = Bird.transform.eulerAngles.y;
        cameraContainer.transform.eulerAngles = desiredRotation;

        /*Vector3 targetDir = Bird.transform.position;

        // The step size is equal to speed times frame time.
        float step = 5.0f * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(cameraContainer.transform.forward, targetDir, step, 1.0f);
        Debug.DrawRay(transform.position, newDir, Color.red);

        // Move our position a step closer to the target.
        cameraContainer.transform.rotation = Quaternion.LookRotation(newDir);*/
    }
    protected void InputChecks()
    {
        //checks up and down input
        TurnUpCheck();
        TurnDownCheck();

        //checks left and right
        TurnLeftCheck();
        TurnRightCheck();

        //Test if the player is walking
        WalkingCheck();
    }

    protected void OutputOrientation()
    {        
        //up & down
        if (FlyingModel.bTurnUp) {
            
            TurnUpAction();

        } else if (FlyingModel.bTurnDown) {

            TurnDownAction();

        } else {
            FlyingModel.pitchValue = 0.0f;
            NoPitchAction();
        }

        //left & right
        if (FlyingModel.bTurnLeft) {

            TurnLeftAction();

        } else if (FlyingModel.bTurnRight) {

            TurnRightAction();

        } else {
            FlyingModel.rollValue = 0.0f;
        }

        //set player orientation instantly
        //Bird.transform.eulerAngles = new Vector3(FlyingModel.pitchValue, FlyingModel.yawValue, FlyingModel.rollValue);

        //lerp player orientation
        Bird.transform.rotation = Quaternion.Lerp(Bird.transform.rotation, Quaternion.Euler(FlyingModel.pitchValue, FlyingModel.yawValue, FlyingModel.rollValue), 0.025f);
    }

    /*
     * Input Checks
     */

    protected void TurnUpCheck()
    {
        //if both the controllers are being rolled up
        if (RC.transform.eulerAngles.z > 180 && LC.transform.eulerAngles.z < 180) {

            //if both controllers break the rotational threshold
            if (Mathf.Abs(RC.transform.eulerAngles.z - 360) > FlyingModel.pitchThreshold && Mathf.Abs(LC.transform.eulerAngles.z) > FlyingModel.pitchThreshold) {

                /* turnUp true */
                FlyingModel.bTurnUp = true;


                //set the pitch value 
                ///FlyingModel.pitchValue = (RC.transform.eulerAngles.z + (LC.transform.eulerAngles.z * -1.0f));
                FlyingModel.pitchValue = ((Mathf.Abs(RC.transform.eulerAngles.z - 360) + (LC.transform.eulerAngles.z)) * -1.0f) / 2.0f;

                //if haptic feedback hasn't been trigger
                if (! FlyingModel.bTurnUpHaptic) {
                    FlyingModel.bTurnUpHaptic = true;
                    triggerHapticFeedback();
                }

            } else {
                /* turnUp false */
                FlyingModel.bTurnUp = false;
                FlyingModel.bTurnUpHaptic = false;
            }

        } else {
            /* turnUp false */
            FlyingModel.bTurnUp = false;
            FlyingModel.bTurnUpHaptic = false;
        }
    }

    protected void TurnDownCheck()
    {
        //if both the controllers are being rolled down
        if (RC.transform.eulerAngles.z < 180 && LC.transform.eulerAngles.z > 180) {

            //if both controllers break the rotational threshold
            if (Mathf.Abs(RC.transform.eulerAngles.z) > FlyingModel.pitchThreshold && Mathf.Abs(LC.transform.eulerAngles.z - 360) > FlyingModel.pitchThreshold) {

                /* turnUp true */
                FlyingModel.bTurnDown = true;


                //set the pitch value 
                FlyingModel.pitchValue = ((Mathf.Abs(LC.transform.eulerAngles.z - 360) + RC.transform.eulerAngles.z)) / 2.0f;

                //if haptic feedback hasn't been trigger
                if (! FlyingModel.bTurnDownHaptic) {
                    FlyingModel.bTurnDownHaptic = true;
                    triggerHapticFeedback();
                }

            } else {
                /* turnUp false */
                FlyingModel.bTurnDown = false;
                FlyingModel.bTurnDownHaptic = false;
            }

        } else {
            /* turnUp false */
            FlyingModel.bTurnDown = false;
            FlyingModel.bTurnDownHaptic = false;
        }
    }

    protected void TurnLeftCheck()
    {
        //if both controllers are being rolled to the left
        if (RC.transform.eulerAngles.x > 180 && LC.transform.eulerAngles.x < 180) {

            //if both controllers break the rotational threshold
            if (Mathf.Abs(RC.transform.eulerAngles.x - 360) > FlyingModel.rollThreshold && Mathf.Abs(LC.transform.eulerAngles.x) > FlyingModel.rollThreshold) {
                
                /* turnLeft true */
                FlyingModel.bTurnLeft = true;

                //set the roll value 
                FlyingModel.rollValue = ((Mathf.Abs(RC.transform.eulerAngles.x - 360) + LC.transform.eulerAngles.x)) / 2.0f;

                //if haptic feedback hasn't been trigger
                if (! FlyingModel.bTurnLeftHaptic) {
                    FlyingModel.bTurnLeftHaptic = true;
                    triggerHapticFeedback();
                }

            } else {
                /* turnLeft false */
                FlyingModel.bTurnLeft = false;
                FlyingModel.bTurnLeftHaptic = false;
            }

        } else {
            /* turnLeft false */
            FlyingModel.bTurnLeft = false;
            FlyingModel.bTurnLeftHaptic = false;
        }
    }

    protected void TurnRightCheck()
    {
        //if both controllers are being rolled to the right
        if (RC.transform.eulerAngles.x < 180 && LC.transform.eulerAngles.x > 180) {

            //if both controllers break the rotational threshold
            if (Mathf.Abs(RC.transform.eulerAngles.x) > FlyingModel.rollThreshold && Mathf.Abs(LC.transform.eulerAngles.x - 360) > FlyingModel.rollThreshold) {
                
                /* turnRight true */
                FlyingModel.bTurnRight = true;

                //set the roll value 
                FlyingModel.rollValue = (((Mathf.Abs(LC.transform.eulerAngles.x - 360) + RC.transform.eulerAngles.x)) * -1.0f) / 2.0f;

                //if haptic feedback hasn't been trigger
                if (! FlyingModel.bTurnRightHaptic) {
                    FlyingModel.bTurnRightHaptic = true;
                    triggerHapticFeedback();
                }

            } else {
                /* turnRight false */
                FlyingModel.bTurnRight = false;
                FlyingModel.bTurnRightHaptic = false;
            }

        } else {
            /* turnRight false */
            FlyingModel.bTurnRight = false;
            FlyingModel.bTurnRightHaptic = false;
        }
    }

    protected void FlapWingsCheck()
    {
        //arms are up
        if (LC.transform.eulerAngles.x > 180 && RC.transform.eulerAngles.x > 180) {
            
            //if both controllers break the wing flap threshold
            if (Mathf.Abs(LC.transform.eulerAngles.x - 360) > FlyingModel.wingFlapThreshold && Mathf.Abs(RC.transform.eulerAngles.x - 360) > FlyingModel.wingFlapThreshold) {

                /* flapping up true */
                FlyingModel.bFlappingUp = true;
                FlyingModel.bLerpFlap = true;
                FlyingModel.birdFlapStartPosition = Bird.transform.position;

                //if the current flap type doesn't reflect upward flap as last previous move
                if (FlyingModel.currentFlapType != 0) {
                    //set current flap type to 0
                    FlyingModel.currentFlapType = 0;
                    /* Flapping wings false */
                    FlyingModel.bFlappingWings = false;
                }
                

            } else {
                /* flapping up false */
                FlyingModel.bFlappingUp = false;
            }


        //arms are down
        } else if (LC.transform.eulerAngles.x < 180 && RC.transform.eulerAngles.x < 180) {
            
            //if both controllers break the wing flap threshold
            if ((LC.transform.eulerAngles.x) > FlyingModel.wingFlapThreshold && (RC.transform.eulerAngles.x) > FlyingModel.wingFlapThreshold) {

                /* flapping down true */
                FlyingModel.bFlappingDown = true;

                //if the current flap type doesn't reflect downward flap as last previous move
                if (FlyingModel.currentFlapType != 1) {
                    //set current flap type to 1
                    FlyingModel.currentFlapType = 1;

                    /* Flapping wings true */
                    FlyingModel.bFlappingWings = true;

                }

            } else {
                /* flapping down false */
                FlyingModel.bFlappingDown = false;
            }

        //arms are semi-flat in a T-pose
        } else {
            //if the current flap type doesn't reflect upward flap as last previous move
            if (FlyingModel.currentFlapType != 0) {
                //set current flap type to 0
                FlyingModel.currentFlapType = 0;
                /* Flapping wings false */
                FlyingModel.bFlappingWings = false;
            }
        }
    }

    protected void WalkingCheck()
    {

    }

    /*
     * Actions
     */

    protected void ApplyGravity()
    {
        //Slowly move the player down on the y axis
        Vector3 moveDirection = Vector3.zero;
		moveDirection.y -= 0.0015f;
		BirdController.Move(moveDirection);
    }
    protected void TurnUpAction()
    {
        //decrease speed
        float newSpeed = 0.0f;

        //FlyingModel.flyingSpeed = (newSpeed < 0) ? -0.5f : newSpeed;

        //speed is slower than 0
        if (FlyingModel.flyingSpeed < 0 && (FlyingModel.flyingSpeed > FlyingModel.minSpeed)) {
            
            //increase the deceleration
            newSpeed = FlyingModel.flyingSpeed - FlyingModel.accelerationSpeedMultipler;
            FlyingModel.flyingSpeed = newSpeed;

        //new speed is slower than minimum speed
        } else if (FlyingModel.flyingSpeed < FlyingModel.minSpeed) {

            //set speed to minimum speed
            FlyingModel.flyingSpeed = FlyingModel.minSpeed;

        } else {

            //decelerate flying speed by normal amount
            newSpeed = FlyingModel.flyingSpeed + FlyingModel.decelerationSpeedMultipler;
            FlyingModel.flyingSpeed = newSpeed;

        }
    }

    protected void TurnDownAction()
    {
        //increase speed
        float newSpeed = FlyingModel.flyingSpeed + FlyingModel.accelerationSpeedMultipler;
        FlyingModel.flyingSpeed = (newSpeed > FlyingModel.maxSpeed) ? FlyingModel.maxSpeed : newSpeed;
    }

    /* When the player goes back to being level after either flying up or diving down */
    protected void NoPitchAction()
    {
        float newSpeed = 0.0f;

        //Going faster than originally so they need to slowly decrease their speed to minimum gliding speed
        if (FlyingModel.flyingSpeed > FlyingModel.originalFlyingSpeed) {

            //decrease speed
            newSpeed = FlyingModel.flyingSpeed + FlyingModel.decelerationSpeedMultipler;
            FlyingModel.flyingSpeed = newSpeed;

        //Going slower than originally so they need to slowly increase their speed to minimum gliding speed
        } else if (FlyingModel.flyingSpeed < FlyingModel.originalFlyingSpeed) {

            //increase speed
            newSpeed = FlyingModel.flyingSpeed + FlyingModel.accelerationSpeedMultipler;
            FlyingModel.flyingSpeed = newSpeed;
        }
    }

    protected void TurnLeftAction()
    {
        FlyingModel.yawValue -= 1.20f * (Mathf.Abs(FlyingModel.rollValue) / 100);
    }

    protected void TurnRightAction()
    {
        FlyingModel.yawValue += 1.20f * (Mathf.Abs(FlyingModel.rollValue) / 100);
    }

    protected void FlapWingsAction()
    {
        //Start lerping the flap
        Vector3 desiredPosition = FlyingModel.birdFlapStartPosition;
        desiredPosition.y += 0.5f;
        Bird.transform.position = Vector3.Lerp(Bird.transform.position, desiredPosition, 0.1f * Time.deltaTime);

        //flap lerp over
        if (Bird.transform.position.y >= desiredPosition.y) {
            FlyingModel.bLerpFlap = false;
        }
        //Vector3 moveDirection = Vector3.zero;
		//moveDirection.y += 0.5f;
		//BirdController.Move(moveDirection);
    }

    protected void FlyForward()
    {
        Bird.transform.Translate(Vector3.forward * FlyingModel.flyingSpeed * Time.deltaTime);
    }

    protected void triggerHapticFeedback()
    {
        //OVRInput.SetControllerVibration (0.5f, 0.5f, OVRInput.Controller.RTouch);
        //OVRInput.SetControllerVibration (0.5f, 0.5f, OVRInput.Controller.LTouch);
    }

}