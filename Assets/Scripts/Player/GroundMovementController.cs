using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovementController : MonoBehaviour {

	protected GroundMovementModel groundMovementModel;
	protected BirdMovementController birdMovementController;

	// Use this for initialization
	void Start () {
		groundMovementModel = GetComponent<GroundMovementModel>();
		birdMovementController = GetComponent<BirdMovementController>();


		groundMovementModel.timer = groundMovementModel.timeForMoveThreshHold;
	}
	
	// Update is called once per frame
	void Update () {
		//if the player isn't grounded, ignore everything else
		if (! birdMovementController.playerController.isGrounded) {
			return;
		}

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
			birdMovementController.moveWhereLooking();
		} else {
			//if there isn't continuous movement of the arms, then reset everything
			resetAll();
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
				resetTimer();
			}	

		//if the right arm was previously higher
		} else if (groundMovementModel.rightArmHigher) {
			
			if (getIsLeftArmHigher()) {
				setLeftArmHigher();
				//arm movement switched to left
				resetTimer();
			}

		}

		countTimerDown();


		if (groundMovementModel.timer > 0) {
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
}