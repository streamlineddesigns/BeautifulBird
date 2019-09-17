using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovementController : MonoBehaviour {

	protected GroundMovementModel groundMovementModel;
	public GameObject player;
	public CharacterController playerController;
	public GameObject playerLookingDirection;
	public GameObject cameraRig;
	public GameObject RC;
	public GameObject LC;
	public GameObject walkingCamera;
	public GameObject flyingCamera;
	public GameObject BirdContainer;
	public GameObject Bird;
	protected float speedModifier;
	public FlyingModel FlyingModel;
	public float orginalJumpTimer;
	public float jumpTimer;
	public float GravityForce;
	public float OriginalGravity;

	// Use this for initialization
	void Start () {
		groundMovementModel = GetComponent<GroundMovementModel>();
		groundMovementModel.timer = groundMovementModel.timeForMoveThreshHold;

		groundMovementModel.rotationThreshold = 45.0f;
		groundMovementModel.rotationSpeed = 0.30f;

		groundMovementModel.armMovementThreshold = 20.0f;

		orginalJumpTimer = 0.5f;
		jumpTimer = orginalJumpTimer;

		OriginalGravity = 0.075f;
		GravityForce = OriginalGravity;
	}
	
	// Update is called once per frame
	void Update () {
		//if the player is on the ground and they're using the ground controller
		if (Feet.Singleton.grounded && GameController.Singleton.controllerSwitch == 1) {
			//check if the player jumps
			JumpCheck();

		//If the player isn't on the ground, but is still using this ground movement controller
		} else if (! Feet.Singleton.grounded && GameController.Singleton.controllerSwitch == 1) {
			//animated jump
			if (groundMovementModel.bJumping) {
				GravityForce = OriginalGravity;
				Jump();
			} else {
				jumpTimer = orginalJumpTimer;
				//Start falling
				Fall();
			}
		}

		//check to see if the player started flying
		if (GameController.Singleton.controllerSwitch != 1) {

			if (groundMovementModel.bSwitchFromFlying) {
                groundMovementModel.bSwitchFromFlying = false;
            }

            return;
        }

		//player started walking after being in the air
        if (! groundMovementModel.bSwitchFromFlying) {
            groundMovementModel.bSwitchFromFlying = true;
            //play normal walking animation
            FlyingModel.BirdAnimator.SetBool("Fly", false);
            FlyingModel.BirdAnimator.SetFloat("Vertical", 0.0f);
        }

		rotationalCheck();
		

		//if one of the arms isn't higher than the other
		if (! groundMovementModel.armPositionSet) {
			checkForArmHeightDifference();
			return;
		}

		//if the initial arms opposite movmement motions haven't occured
		if (! groundMovementModel.initialArmMovement) {
			checkForInitialArmMovement();
			return;
		}

		if (checkForContinuousArmMovement()) {
			//if continuous movement of the arms is occuring,  move forward
			moveWhereLooking();
		} else {
			//if there isn't continuous movement of the arms, then reset everything
			resetAll();
		}
	}

	protected void rotationalCheck()
	{
		//Using local rotation instead
		//Debug.Log(playerLookingDirection.transform.localRotation.eulerAngles.y);
		//if the players looking direction is greater than the rotational threshold
		if (playerLookingDirection.transform.localRotation.eulerAngles.y >= 180) {
			
			if (Mathf.Abs(playerLookingDirection.transform.localRotation.eulerAngles.y - 360) >= groundMovementModel.rotationThreshold) {
				groundMovementModel.bCurrentlyRotating = true;
				//left rotation
				rotateLeftAction();
				//Debug.Log("rotating left");
			}

		//if the players looking direction is less than the rotational threshold
		} else if (playerLookingDirection.transform.localRotation.eulerAngles.y <= 180) {

			if (playerLookingDirection.transform.localRotation.eulerAngles.y >= groundMovementModel.rotationThreshold) {
				groundMovementModel.bCurrentlyRotating = true;
				//right rotation
				rotateRightAction();
				//Debug.Log("rotating right");
			}

		} else {
			groundMovementModel.bCurrentlyRotating = false;
		}
	}

	protected void JumpCheck()
    {

        //arms are up
        if (LC.transform.eulerAngles.x > 180 && RC.transform.eulerAngles.x > 180) {
            
            //if both controllers break the jump threshold
            if (Mathf.Abs(LC.transform.eulerAngles.x - 360) > groundMovementModel.armMovementThreshold && Mathf.Abs(RC.transform.eulerAngles.x - 360) > groundMovementModel.armMovementThreshold) {

                if (! groundMovementModel.bJumping) {
					/* jumping true */
                    groundMovementModel.bJumping = true;
					JumpInit();
                }

            } else {
                /* jumping false */
                groundMovementModel.bJumping = false;
            }

        } else {
            if (!groundMovementModel.bJumping) {
                /* jumping false */
                groundMovementModel.bJumping = false;
            }
        }
    }

	protected void JumpInit()
	{
		//move the player up
		BirdContainer.transform.Translate(BirdContainer.transform.up * 0.1f);
	}
	protected void Jump()
	{
		if (jumpTimer >= 0.0f) {
			jumpTimer -= Time.deltaTime;
			//move the player up a little bit
			BirdContainer.transform.Translate(BirdContainer.transform.up * 1.0f * Time.deltaTime);
			//also move forward
			moveWhereLooking();
		} else {
			groundMovementModel.bJumping = false;
		}
	}

	protected void checkForArmHeightDifference()
	{
		if (getIsLeftArmHigher()) {

			setLeftArmHigher();
			//arm positions set
			groundMovementModel.armPositionSet = true;

		} else if (getIsRightArmHigher()) {

			setRightArmHigher();
			//arm positions set
			groundMovementModel.armPositionSet = true;
		}
	}

	protected void checkForInitialArmMovement() {
		//if the left arm was previously higher
		if (groundMovementModel.leftArmHigher) {

			if (getIsRightArmHigher()) {
				groundMovementModel.initialArmMovement = true;
				setRightArmHigher();
			}

		//if the right arm was previously higher
		} else if (groundMovementModel.rightArmHigher) {
			
			if (getIsLeftArmHigher()) {
				groundMovementModel.initialArmMovement = true;
				setLeftArmHigher();
			}

		}
	}

	protected bool checkForContinuousArmMovement()
	{
		//if the left arm was previously higher
		if (groundMovementModel.leftArmHigher) {

			if (getIsRightArmHigher()) {
				setRightArmHigher();
				//arm movement switched to right
				groundMovementModel.speedMultiplier = groundMovementModel.timer;
				resetTimer();
			}	

		//if the right arm was previously higher
		} else if (groundMovementModel.rightArmHigher) {
			
			if (getIsLeftArmHigher()) {
				setLeftArmHigher();
				//arm movement switched to left
				groundMovementModel.speedMultiplier = groundMovementModel.timer;
				resetTimer();
			}

		}

		countTimerDown();


		if (groundMovementModel.timer > 0.2f) {
			return true;
		} else {
			return false;
		}
	}

	protected bool getIsLeftArmHigher()
	{
		Vector3 leftHandCurPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		Vector3 rightHandCurPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

		if (leftHandCurPos.y > rightHandCurPos.y + groundMovementModel.movementThreshHold) {
			speedModifier = leftHandCurPos.y - rightHandCurPos.y + groundMovementModel.movementThreshHold;
			return true;
		} else {
			return false;
		}
	}

	protected bool getIsRightArmHigher()
	{
		Vector3 leftHandCurPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		Vector3 rightHandCurPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

		if (rightHandCurPos.y > leftHandCurPos.y + groundMovementModel.movementThreshHold) {
			speedModifier = rightHandCurPos.y - leftHandCurPos.y + groundMovementModel.movementThreshHold;
			return true;
		} else {
			return false;
		}
	}
	protected void setLeftArmHigher()
	{
		//right arm is lower
		groundMovementModel.rightArmHigher = false;
		//left arm is higher
		groundMovementModel.leftArmHigher = true;
	}

	protected void setRightArmHigher()
	{
		//left arm is lower
		groundMovementModel.leftArmHigher = false;
		//right arm is higher
		groundMovementModel.rightArmHigher = true;
	}

	protected void countTimerDown() {
		groundMovementModel.timer -= Time.deltaTime;
	}
	protected void resetTimer() {
		groundMovementModel.timer = groundMovementModel.timeForMoveThreshHold;
	}
	protected void resetAll()
	{
		groundMovementModel.timer = groundMovementModel.timeForMoveThreshHold;
		groundMovementModel.rightArmHigher = false;
		groundMovementModel.leftArmHigher = false;
		groundMovementModel.armPositionSet = false;
		groundMovementModel.initialArmMovement = false;
	}
	public void moveWhereLooking()
	{
		/* if () {

		} else if () {

		}*/
		//Vector3 accelerationVector = player.transform.TransformDirection(Bird.transform.forward * (1.0f + speedModifier) * Time.deltaTime);
		//playerController.Move(accelerationVector);

		//BirdContainer.transform.Translate(cameraRig.transform.forward * (1.0f + speedModifier) * Time.deltaTime);
		//Bird.transform.Translate(BirdContainer.transform.forward * (1.0f + speedModifier) * Time.deltaTime);
		//BirdContainer.transform.Translate(Bird.transform.forward * (1.0f + speedModifier) * Time.deltaTime);
		BirdContainer.transform.Translate(cameraRig.transform.forward * (1.0f + speedModifier) * Time.deltaTime);
	}

	public void Fall()
	{
		//if the model doesn't have it set that the player is falling
		if (! groundMovementModel.bIsFalling) {
			//set it to falling
			groundMovementModel.bIsFalling = true;
		}
		
		//if the gravity is less than 400% the original gravity
		if (GravityForce < OriginalGravity * 4.0f) {
			//add more gravity
			GravityForce += OriginalGravity / 100.0f;
		}

        //move bird container down 
        BirdContainer.transform.Translate(- BirdContainer.transform.up * GravityForce * 20.0f * Time.deltaTime);
	}

	protected void rotateLeftAction()
    {
		Vector3 targetRotation;
		targetRotation = Bird.transform.eulerAngles;
        targetRotation.y -= groundMovementModel.rotationSpeed;
		FlyingModel.yawValue -= groundMovementModel.rotationSpeed;
		Bird.transform.eulerAngles = targetRotation;
    }

    protected void rotateRightAction()
    {
		Vector3 targetRotation;
		targetRotation = Bird.transform.eulerAngles;
        targetRotation.y += groundMovementModel.rotationSpeed;
		FlyingModel.yawValue += groundMovementModel.rotationSpeed;
		Bird.transform.eulerAngles = targetRotation;
    }
}