using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flapping : MonoBehaviour {
    public GameObject LC;
    public GameObject RC;
    public GameObject CameraContainer;
    public GameObject BirdContainer;
    public GameObject Bird;
    public Collider BirdFeetCollider;
    protected FlyingModel FlyingModel;
    public GameObject CamTarget;
    public GameObject ExtendedWingTrails;
    public GameObject FasterSpeedTrails;
    public GameObject MediumSpeedTrails;
    public float GravityForce;
    public float OriginalGravity;


    void Awake() 
    {
        
    }
    void Start() 
    {
        //model
        FlyingModel = GetComponent<FlyingModel>();

        //time
        FlyingModel.initialTime = 0.77f;
        FlyingModel.timeLeft = FlyingModel.initialTime;

        //thresholds
        FlyingModel.pitchThreshold = 1.0f;
        FlyingModel.rollThreshold = 1.0f;
        FlyingModel.wingFlapThreshold = 20.0f;
        FlyingModel.diveThreshold = 45.0f;

        //flapping
        FlyingModel.flapHeightIncrease = 2.0f;

        //Speeds
        //Speed
        FlyingModel.originalFlyingSpeed = 2.0f;
        FlyingModel.flyingSpeed = FlyingModel.originalFlyingSpeed;
        FlyingModel.maxSpeed = 6.0f;
        FlyingModel.minSpeed = 0.0f;
        FlyingModel.decelerationSpeedMultipler = 0.0075f;
        FlyingModel.accelerationSpeedMultipler = 0.0075f;

        GravityForce = 0.0075f;
        OriginalGravity = GravityForce;
    }

    void Update() 
    {
        /*if (IsGrounded()) {
            return;
        }*/
        //Gravity
        //Add Gravity to the bird
        ApplyGravity();
        //Fly forward constantly now
        FlyForward();

        TurnUpCheck();
        TurnDownCheck();
        TurnLeftCheck();
        TurnRightCheck();

        /* TURNING UP/DOWN */

        //up & down
        if (FlyingModel.bTurnUp) {
            
            TurnUpAction();

        } else if (FlyingModel.bTurnDown) {

            TurnDownAction();

        } else {
            FlyingModel.BirdAnimator.SetFloat("UpDown", 0.0f);
            FlyingModel.pitchValue = 0.0f;
            GlidAction();
            FlyingModel.bDiving = false;

            //dont let the player go over max speed
            if (FlyingModel.flyingSpeed > FlyingModel.maxSpeed) {
                FlyingModel.flyingSpeed = FlyingModel.maxSpeed;
            }
        }

        /* Banking LEFT/RIGHT */

        //left & right
        if (FlyingModel.bTurnLeft) {

            TurnLeftAction();

        } else if (FlyingModel.bTurnRight) {

            TurnRightAction();

        } else {
            FlyingModel.rollValue = 0.0f;
        }

        /* Flapping */

        //if the player isn't flapping their wings
        if (! FlyingModel.bFlappingWings) {

            
            //check if they're flapping their wings
            FlapWingsCheck();

        } else {

            //if the flapping animation isn't playing
            if (FlyingModel.BirdAnimator.GetFloat("Vertical") != 1.0f) {

                //play flapping animation
                FlyingModel.BirdAnimator.SetFloat("Vertical", 1.0f);
                
                if (FlyingModel.flyingSpeed <= FlyingModel.minSpeed) {

                    //5 flaps to get back to minimum speed
                    FlyingModel.flyingSpeed+= FlyingModel.minSpeed / 5.0f;

                } else if (FlyingModel.flyingSpeed < FlyingModel.maxSpeed) {
                    //7 flaps to get back to max speed
                    FlyingModel.flyingSpeed+= FlyingModel.maxSpeed / 7.0f;
                }
                if (GravityForce < OriginalGravity) {
                    //3 flaps to stop falling
                    GravityForce = OriginalGravity;
                }

            //if the flapping animation is playing
            } else {

                //If theres more time left than 0 seconds
                if (FlyingModel.timeLeft > 0) {

                    //Subtract the time
                    FlyingModel.timeLeft -= Time.deltaTime;

                    //Move the bird upwards while the timer is going
                    Vector3 newPos = new Vector3(BirdContainer.transform.position.x, BirdContainer.transform.position.y, BirdContainer.transform.position.z);
                    newPos.y += FlyingModel.flapHeightIncrease;
                    BirdContainer.transform.position = Vector3.Lerp(BirdContainer.transform.position, newPos, Time.deltaTime * 1.5f);

                //The time has ran out
                } else {
                    
                    //the birds wings need to stop flapping
                    FlyingModel.BirdAnimator.SetBool("Fly", false);
                    FlyingModel.BirdAnimator.SetFloat("Vertical", 2.0f);
                    FlyingModel.bFlappingWings = false;
                    FlyingModel.BirdAnimator.SetBool("Fly", true);

                    //Reset the timer
                    FlyingModel.timeLeft = FlyingModel.initialTime;
                }
            }
        }

        //Set the birds orientation
        Bird.transform.rotation = Quaternion.Lerp(Bird.transform.rotation, Quaternion.Euler(FlyingModel.pitchValue, FlyingModel.yawValue, FlyingModel.rollValue), 0.1f);
        
        //Camera follows bird
        Vector3 newPos2 = new Vector3(CamTarget.transform.position.x, CameraContainer.transform.position.y, CamTarget.transform.position.z);
        //The bird is flapping
        if (FlyingModel.BirdAnimator.GetFloat("Vertical") == 1.0f) {
            //move x & z normal speed and dont move y
            newPos2.y = CameraContainer.transform.position.y;
            CameraContainer.transform.position = Vector3.Lerp(CameraContainer.transform.position, newPos2, Time.deltaTime * 4.5f);
            //move y slow
            newPos2.y = CamTarget.transform.position.y;
            CameraContainer.transform.position = Vector3.Lerp(CameraContainer.transform.position, newPos2, Time.deltaTime * 3.5f);
        //the bird isn't flapping
        } else {
            CameraContainer.transform.position = Vector3.Lerp(CameraContainer.transform.position, CamTarget.transform.position, Time.deltaTime * 4.5f);
        }

        //Set Rotation
        Vector3 desiredRotation;
        desiredRotation = CameraContainer.transform.eulerAngles;
        desiredRotation.y = Bird.transform.eulerAngles.y;
        CameraContainer.transform.eulerAngles = desiredRotation;

        //if the bird is flapping wings or diving
        if ((FlyingModel.BirdAnimator.GetFloat("Vertical") == 1.0f) || FlyingModel.bDiving) {
            //if the wing trails are on
            if (ExtendedWingTrails.activeSelf) {
                //set them off
                ExtendedWingTrails.SetActive(false);
            }
        } else {
            if (! ExtendedWingTrails.activeSelf) {
                //set them off
                FasterSpeedTrails.SetActive(false);
                MediumSpeedTrails.SetActive(false);
                ExtendedWingTrails.SetActive(true);
            }
        }
    }

    protected bool IsGrounded()
	{
		//Ray takes an original position, and a direction
		Ray groundRay = new Ray(
			new Vector3(
				//Position
				BirdFeetCollider.bounds.center.x,
				(BirdFeetCollider.bounds.center.y - BirdFeetCollider.bounds.extents.y) + 0.2f,
				BirdFeetCollider.bounds.center.z
			),
			//Direction
			Vector3.down
		);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);
		return Physics.Raycast(groundRay, 0.2f + 0.1f);
	}
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

    protected void ApplyGravity()
    {
        //The player is FALLING
        if (FlyingModel.flyingSpeed <= 0) {
            //if the force of gravity is less than 150% the max speed
            if (GravityForce < FlyingModel.maxSpeed * 1.5) {
                //fall 5 times faster than usual
                GravityForce += FlyingModel.decelerationSpeedMultipler * 3;
            }
        } else {
            GravityForce = OriginalGravity;
        }
        //gravity 
        BirdContainer.transform.Translate(- BirdContainer.transform.up * GravityForce * Time.deltaTime);
    }

    protected void FlyForward()
    {
        BirdContainer.transform.Translate(Bird.transform.forward * FlyingModel.flyingSpeed * Time.deltaTime);
    }

    protected void TurnLeftAction()
    {
        FlyingModel.yawValue -= 1.20f * (Mathf.Abs(FlyingModel.rollValue) / 100);
    }

    protected void TurnRightAction()
    {
        FlyingModel.yawValue += 1.20f * (Mathf.Abs(FlyingModel.rollValue) / 100);
    }

    protected void TurnUpAction()
    {
        //decrease speed
        float newSpeed = 0.0f;

        //FlyingModel.flyingSpeed = (newSpeed < 0) ? -0.5f : newSpeed;

        //speed is slower than 0
        if (FlyingModel.flyingSpeed < FlyingModel.minSpeed) {

            //set speed to minimum speed
            FlyingModel.flyingSpeed = FlyingModel.minSpeed;

        } else {

            //decelerate flying speed by normal amount
            newSpeed = FlyingModel.flyingSpeed - FlyingModel.decelerationSpeedMultipler;
            FlyingModel.flyingSpeed = newSpeed;

        }
    }
    protected void TurnDownAction()
    {
        //increase speed
        float newSpeed = FlyingModel.flyingSpeed + FlyingModel.accelerationSpeedMultipler;
        FlyingModel.flyingSpeed = (newSpeed > FlyingModel.maxSpeed) ? FlyingModel.maxSpeed : newSpeed;

        //diving
        if (Mathf.Abs(RC.transform.eulerAngles.z) > FlyingModel.diveThreshold && Mathf.Abs(LC.transform.eulerAngles.z - 360) > FlyingModel.diveThreshold) {
            //diving fast
            if (FlyingModel.flyingSpeed >= FlyingModel.maxSpeed) {
                FlyingModel.bDiving = true;
                FlyingModel.BirdAnimator.SetFloat("UpDown", -2.0f);
                MediumSpeedTrails.SetActive(false);
                FasterSpeedTrails.SetActive(true);

            //diving 3/4 speed
            } else if (FlyingModel.flyingSpeed >= FlyingModel.maxSpeed * .75f) {
                FlyingModel.bDiving = true;
                FlyingModel.BirdAnimator.SetFloat("UpDown", -1.0f);
                FasterSpeedTrails.SetActive(false);
                MediumSpeedTrails.SetActive(true);
            } else {
                FlyingModel.bDiving = false;
                FasterSpeedTrails.SetActive(false);
                MediumSpeedTrails.SetActive(false);
            }
        //not going downard enough to be diving
        } else {
            FlyingModel.bDiving = false;
            if (FlyingModel.BirdAnimator.GetFloat("UpDown") != 0.0f) {
                FlyingModel.BirdAnimator.SetFloat("UpDown", 0.0f);
            }

        }
    }

    /* When the player goes back to being level after either flying up or diving down */
    protected void GlidAction()
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

    protected void triggerHapticFeedback()
    {
        //OVRInput.SetControllerVibration (0.5f, 0.5f, OVRInput.Controller.RTouch);
        //OVRInput.SetControllerVibration (0.5f, 0.5f, OVRInput.Controller.LTouch);
    }
}