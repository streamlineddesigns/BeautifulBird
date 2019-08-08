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

	// Use this for initialization
	void Start () {
		groundMovementModel = GetComponent<GroundMovementModel>();
		groundMovementModel.timer = groundMovementModel.timeForMoveThreshHold;

		groundMovementModel.rotationThreshold = 45.0f;
		groundMovementModel.rotationSpeed = 0.75f;

		groundMovementModel.armMovementThreshold = 20.0f;
	}
	
	// Update is called once per frame
	void Update () {
		//if the player isn't grounded, ignore everything else
		if (! playerController.isGrounded) {
			Fall();
			return;
		}

		rotationalCheck();
		JumpCheck();

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
					Jump();
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

	protected void Jump()
	{
		Vector3 moveDirection = Vector3.zero;
		moveDirection.y += 1.5f;
		playerController.Move(moveDirection);
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
		Vector3 accelerationVector = player.transform.TransformDirection(playerLookingDirection.transform.forward) * (0.055f * groundMovementModel.speedMultiplier);
		playerController.Move(accelerationVector);
	}

	public void Fall()
	{
		Vector3 accelerationVector = player.transform.TransformDirection(playerLookingDirection.transform.forward) * (0.055f * groundMovementModel.speedMultiplier);
		accelerationVector.y -= 0.025f;
		playerController.Move(accelerationVector);
		//Vector3 moveDirection = Vector3.zero;
		//moveDirection.y -= 0.01f;
		//playerController.Move(moveDirection);
	}

	protected void rotateLeftAction()
    {
		Vector3 targetRotation;
		targetRotation = cameraRig.transform.eulerAngles;
        targetRotation.y -= groundMovementModel.rotationSpeed;
		cameraRig.transform.eulerAngles = targetRotation;
    }

    protected void rotateRightAction()
    {
		Vector3 targetRotation;
		targetRotation = cameraRig.transform.eulerAngles;
        targetRotation.y += groundMovementModel.rotationSpeed;
		cameraRig.transform.eulerAngles = targetRotation;
    }
}