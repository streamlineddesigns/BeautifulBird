using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flapping : MonoBehaviour {
    public GameObject LC;
    public GameObject RC;
    public GameObject CameraContainer;
    public GameObject Camera;
    public GameObject BirdContainer;
    public GameObject Bird;
    protected CharacterController BirdController;
    public Collider BirdFeetCollider;
    protected FlyingModel FlyingModel;
    public GameObject CamTarget;
    public GameObject ExtendedWingTrails;
    public GameObject FasterSpeedTrails;
    public GameObject MediumSpeedTrails;
    public float GravityForce;
    public float OriginalGravity;
    public GameObject WarpDrive;
    protected ParticleSystem WarpDriveParticles;
    public GameObject warningToFlapText;
    public GameObject lookingDirection;
    public float upModifier;
    public CharacterController playerController;
    public GameObject flyingCamera;
    public GroundMovementModel GroundMovementModel;


    void Awake() 
    {
        Application.targetFrameRate = 90;
    }
    void Start() 
    {
        //model
        FlyingModel = GetComponent<FlyingModel>();

        //time
        FlyingModel.initialTime = 0.77f;
        FlyingModel.timeLeft = FlyingModel.initialTime;

        //thresholds
        FlyingModel.pitchThreshold = 5.0f;
        FlyingModel.rollThreshold = 7.5f;
        FlyingModel.wingFlapThreshold = 5.0f;//this is rotation
        FlyingModel.controllerFlapThreshold = 0.05f;//this is height
        FlyingModel.diveThreshold = 50.0f;

        //flapping
        FlyingModel.flapHeightIncrease = 2.0f;

        //Speeds
        //Speed
        FlyingModel.originalFlyingSpeed = 2.5f;
        FlyingModel.flyingSpeed = FlyingModel.originalFlyingSpeed;
        FlyingModel.maxSpeed = 7.0f;
        FlyingModel.originalMaxSpeed = 7.0f;
        FlyingModel.minSpeed = 0.0f;
        FlyingModel.decelerationSpeedMultipler = 0.008f;
        FlyingModel.accelerationSpeedMultipler = 0.008f;

        GravityForce = 0.0075f;
        OriginalGravity = GravityForce;
        WarpDriveParticles = GetComponent<ParticleSystem>();
        BirdController = Bird.GetComponent<CharacterController>();

    }

    void Update() 
    {

        //if groundMovementModel.bIsFalling
        /*
        
        //if the player isn't flapping their wings
        if (! FlyingModel.bFlappingWings) {

            //FlapWingsCheck();
            //if both of the grips are being pressed
			if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0.0f &&
				OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.0f) {
					//check if they're flapping their wings
                    FlapWingsCheck();
			}
            

        //if the player is flapping their wings
        }
        
         */


        /*
            //this allows players to flap their wings when they are groundmovement->falling or groundmovement->jumping
         */

        //if the player is in walking mode
        if (GameController.Singleton.controllerSwitch == 1) {
            //if the player is fallin or jumping
            if (GroundMovementModel.bIsFalling || GroundMovementModel.bJumping) {
                //if the player isn't flapping their wings
                //if (! FlyingModel.bFlappingWings) {
                    //if both of the grips are being pressed
                    if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0.0f && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.0f) {
                        //check if they're flapping their wings
                        FlapWingsCheck();
                        //if they are flapping their wings
                        if (FlyingModel.bFlappingWings) {
                            //Fly time
                            GameController.Singleton.Fly();
                        }
                    }
                //}
            }
        }
        
        if (GameController.Singleton.controllerSwitch != 0) {


            //reset the speed if the player is walking
            if (FlyingModel.flyingSpeed > 0) {
                FlyingModel.flyingSpeed = 0;
            }

            //reset the switch
            if (FlyingModel.bSwitchFromGround) {
                FlyingModel.bSwitchFromGround = false;
                //reset the pitch
                if (Bird.transform.rotation.eulerAngles.x != 0.0f) {
                    FlyingModel.pitchValue = 0.0f;
                    //Bird.transform.rotation = new Vector3(FlyingModel.pitchValue, Bird.transform.rotation.eulerAngles.y, Bird.transform.rotation.eulerAngles.z);
                    Bird.transform.rotation = Quaternion.Euler(FlyingModel.pitchValue, Bird.transform.rotation.eulerAngles.y, Bird.transform.rotation.eulerAngles.z);
                    
                    //to make sure they're in the ground
                    BirdContainer.transform.position = new Vector3(BirdContainer.transform.position.x, BirdContainer.transform.position.y - 0.025f, BirdContainer.transform.position.z);
                
                    //Hide the fast wind streaks from in front
                    FlyingModel.minWind.SetActive(false);
                    FlyingModel.medWind.SetActive(false);
                    FlyingModel.maxWind.SetActive(false);

                    //no more wind sound
                    FlyingModel.WindSound.volume = 0.0f;

                    //no more wing trails
                    FasterSpeedTrails.SetActive(false);
                    MediumSpeedTrails.SetActive(false);
                    ExtendedWingTrails.SetActive(false);
                }

                //make sure flap text is not there
                warningToFlapText.SetActive(false);
            }

            return;
        }


        //player started flying after being on the ground
        if (! FlyingModel.bSwitchFromGround) {
            FlyingModel.bSwitchFromGround = true;
            //play normal flying animation
            FlyingModel.BirdAnimator.SetBool("Fly", true);
            FlyingModel.BirdAnimator.SetFloat("Vertical", 2.0f);
        }

        //Debug.Log(FlyingModel.rotationalSpeed);
        //Debug.Log(RC.transform.eulerAngles.x);
        //Debug.Log(Mathf.Abs(RC.transform.eulerAngles.x - 360));
        //Debug.Log(FlyingModel.pitchValue);        
        /* if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) {
            FlyingModel.bisLanding = true;
        }

        if (FlyingModel.bisLanding) {
            if (FlyingModel.flyingSpeed != 0) {
                FlyingModel.flyingSpeed = 0;
            }
            ApplyGravity();
            CameraFollow();
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
            upModifier = 0.0f;
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

            //FlapWingsCheck();
            //if both of the grips are being pressed
			if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0.0f &&
				OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.0f) {
					//check if they're flapping their wings
                    FlapWingsCheck();
			}
            

        //if the player is flapping their wings
        } else {

            //if the flapping animation isn't playing
            if (FlyingModel.BirdAnimator.GetFloat("Vertical") != 1.0f) {

                //play flapping animation
                FlyingModel.BirdAnimator.SetFloat("Vertical", 1.0f);
                
                if (FlyingModel.flyingSpeed < FlyingModel.originalFlyingSpeed) {

                    //1 flaps to get back to original speed
                    FlyingModel.flyingSpeed+= FlyingModel.originalFlyingSpeed;

                } else if (FlyingModel.flyingSpeed < FlyingModel.maxSpeed) {
                    //5 flaps to get back to max speed
                    FlyingModel.flyingSpeed+= FlyingModel.maxSpeed / 5.0f;
                }
                
                if (GravityForce > OriginalGravity) {
                    //1 flap to stop falling so fast
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
        Bird.transform.rotation = Quaternion.Lerp(Bird.transform.rotation, Quaternion.Euler(FlyingModel.pitchValue, FlyingModel.yawValue, FlyingModel.rollValue), 0.02f);
        //Bird.transform.rotation = Quaternion.Lerp(Bird.transform.rotation, Quaternion.Euler(FlyingModel.pitchValue, FlyingModel.yawValue, 0.0f), 0.02f);
        CameraFollow();

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

        //if the players speed is less than 30% of its original speed
        if (FlyingModel.flyingSpeed <= FlyingModel.originalFlyingSpeed * 0.30f) {
            warningToFlapText.SetActive(true);
        } else {
            warningToFlapText.SetActive(false);
        }

        //speed trails & sounds
        SetWind();
    }

    protected void SetWind()
    {
        if (FlyingModel.flyingSpeed >= FlyingModel.maxSpeed) {
            FlyingModel.maxWind.SetActive(true);//max
            FlyingModel.medWind.SetActive(false);
            FlyingModel.minWind.SetActive(false);
        } else if (FlyingModel.flyingSpeed >= FlyingModel.maxSpeed * 0.75f) {
            FlyingModel.medWind.SetActive(true);//med
            FlyingModel.maxWind.SetActive(false);
            FlyingModel.minWind.SetActive(false);
        } else if (FlyingModel.flyingSpeed >= FlyingModel.maxSpeed * 0.50f) {
            FlyingModel.minWind.SetActive(true);//min
            FlyingModel.medWind.SetActive(false);
            FlyingModel.maxWind.SetActive(false);
        } else {
            FlyingModel.minWind.SetActive(false);//none
            FlyingModel.medWind.SetActive(false);
            FlyingModel.maxWind.SetActive(false);
        }

        /*if (FlyingModel.flyingSpeed >= 1.0f) {
            FlyingModel.WindSound.volume = FlyingModel.flyingSpeed / 10.0f;
        } else {
            FlyingModel.WindSound.volume = FlyingModel.flyingSpeed / 10.0f;
        }*/
        FlyingModel.WindSound.volume = (FlyingModel.flyingSpeed / 10.0f) - 0.6f;
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

    protected void CameraFollow() {
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
    }
    protected void TurnUpCheck()
    {
        if (RC.transform.eulerAngles.z > 180 && LC.transform.eulerAngles.z < 180 && (Mathf.Abs(RC.transform.eulerAngles.z - 360) > FlyingModel.pitchThreshold && Mathf.Abs(LC.transform.eulerAngles.z) > FlyingModel.pitchThreshold)) {

            //if both controllers break the rotational threshold
            if (Mathf.Abs(RC.transform.eulerAngles.z - 360) > FlyingModel.pitchThreshold && Mathf.Abs(LC.transform.eulerAngles.z) > FlyingModel.pitchThreshold) {

                /* turnUp true */
                FlyingModel.bTurnUp = true;

                
                //set the pitch value 
                //FlyingModel.pitchValue = (RC.transform.eulerAngles.z + (LC.transform.eulerAngles.z * -1.0f));
                //FlyingModel.pitchValue = upModifier + ((Mathf.Abs(RC.transform.eulerAngles.z - 360) + (LC.transform.eulerAngles.z)) * -1.0f) / 2.0f;
                FlyingModel.pitchValue = ((Mathf.Abs(RC.transform.eulerAngles.z - 360) + (LC.transform.eulerAngles.z)) * -1.0f) / 2.0f;
                //Debug.Log(Camera.transform.eulerAngles.x);
                
                //if the headset breaks the rotational threshold, average it with the controllers, otherwise only average the controllers
                //if (((((Camera.transform.eulerAngles.x - 360) - FlyingModel.pitchThreshold) * -1.0f) > 45) && (((Camera.transform.eulerAngles.x - 360) * -1.0f) < 90 )) {
                /*if (lookingDirection.transform.eulerAngles.x > 270 && lookingDirection.transform.eulerAngles.x < 360) {
                    FlyingModel.pitchValue = ( ((lookingDirection.transform.eulerAngles.x - 360)) + (Mathf.Abs(RC.transform.eulerAngles.z - 360) + (LC.transform.eulerAngles.z)) * -1.0f) / 3.0f;
                    
                    //Debug.Log(((lookingDirection.transform.eulerAngles.x - 360) * - 1.0f));
                    //Debug.Log(Mathf.Abs(RC.transform.eulerAngles.z - 360));
                } else {
                    FlyingModel.pitchValue = ((Mathf.Abs(RC.transform.eulerAngles.z - 360) + (LC.transform.eulerAngles.z)) * -1.0f) / 2.0f;
                } */
                
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

        //if the headset breaks a larger rotational threshold
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
                FlyingModel.pitchValue = ((Mathf.Abs(RC.transform.eulerAngles.z - 360) + (LC.transform.eulerAngles.z)) * -1.0f) / 2.0f;
                //FlyingModel.pitchValue += 1.0f;
                
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

                //set the roll value 
                FlyingModel.rollValue = ((Mathf.Abs(RC.transform.eulerAngles.x - 360) + LC.transform.eulerAngles.x)) / 2.0f;

                //set rotation speed
                FlyingModel.rotationalSpeed = 0.3f + ((Mathf.Abs(FlyingModel.rollValue) / FlyingModel.rollThreshold) / 3.0f) * 0.1f;

                /* turnLeft true */
                FlyingModel.bTurnLeft = true;

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

                //set the roll value 
                FlyingModel.rollValue = (((Mathf.Abs(LC.transform.eulerAngles.x - 360) + RC.transform.eulerAngles.x)) * -1.0f) / 2.0f;

                //set rotation speed
                FlyingModel.rotationalSpeed = 0.3f + ((Mathf.Abs(FlyingModel.rollValue) / FlyingModel.rollThreshold) / 3.0f) * 0.1f;

                /* turnRight true */
                FlyingModel.bTurnRight = true;

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
        if (Mathf.Abs(LC.transform.eulerAngles.x - 360) < 90 && Mathf.Abs(RC.transform.eulerAngles.x - 360) < 90) {
            
            //if both controllers break the wing flap threshold
            if (Mathf.Abs(LC.transform.eulerAngles.x - 360) > FlyingModel.wingFlapThreshold && Mathf.Abs(RC.transform.eulerAngles.x - 360) > FlyingModel.wingFlapThreshold) {

                /* flapping up true */
                FlyingModel.bFlappingUp = true;
                FlyingModel.bLerpFlap = true;
                FlyingModel.birdFlapStartPosition = Bird.transform.position;

                FlyingModel.RCStartHeight = RC.transform.position.y;
                FlyingModel.LCStartHeight = LC.transform.position.y;

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
                FlyingModel.RCEndHeight = RC.transform.position.y;
                FlyingModel.LCEndHeight = LC.transform.position.y;

                //if the current flap type doesn't reflect downward flap as last previous move
                if (FlyingModel.currentFlapType != 1) {
                    //set current flap type to 1
                    FlyingModel.currentFlapType = 1;
                    FlyingModel.bFlappingWings = true;
                    /*if ((Mathf.Abs(FlyingModel.RCStartHeight - FlyingModel.RCEndHeight) >= FlyingModel.controllerFlapThreshold) && (Mathf.Abs(FlyingModel.LCStartHeight - FlyingModel.LCEndHeight) >= FlyingModel.controllerFlapThreshold))  {
                        /* Flapping wings true 
                        FlyingModel.bFlappingWings = true;
                    } */

                }

            } else {
                /* flapping down false */
                //FlyingModel.bFlappingDown = false;
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
        FlyingModel.yawValue -= FlyingModel.rotationalSpeed;
    }

    protected void TurnRightAction()
    {
        FlyingModel.yawValue += FlyingModel.rotationalSpeed;
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

/*Flying→user presses “a” button to drop and land onto the ground

the players position is still behind the bird
animation smoothly brings camera into the position of the bird
while this is happening, the bird is becoming a beautiful, glowing orb with spirals of energy coming out of it
when the players camera reaches the bird, the “shimmering” sound plays and the players beak fades into the cameras view
then the player is also able to look down and see their controllers
natural ground traversing would then be possible
 */