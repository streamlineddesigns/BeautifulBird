using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovementController : MonoBehaviour {
	public OVRPlayerController OVRController;
	public GameObject player;
	public CharacterController playerController;
	public GameObject playerLookingDirection;
	public GameObject LeftController;
	public GameObject RightController;
	///public GameObject bird;

	// Use this for initialization
	void Start () {
		
	}


	void Update()
	{
		if (! playerController.isGrounded) {

			moveDown();

			//prevents the player from using the controller to move around
			//Now they have to use their wings
			if (OVRController.EnableLinearMovement) {
				OVRController.EnableLinearMovement = false;
			}
			
			//if both of the grips are being pressed
			if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0.0f &&
				OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.0f) {
					//move the player where the hands are moving
					moveWhereBirdsLooking();
			}
			

		} else if (playerController.isGrounded) {

			if (! OVRController.EnableLinearMovement) {
				OVRController.EnableLinearMovement = true;
			}

		}
	}

	public void moveUp()
	{
		Vector3 moveDirection = Vector3.zero;
		moveDirection.y += 0.5f;
		playerController.Move(moveDirection);
	}

	public void moveWhereLooking()
	{
		Vector3 accelerationVector = player.transform.TransformDirection(playerLookingDirection.transform.forward) * 0.05f;
		playerController.Move(accelerationVector);

	}

	public void moveWhereBirdsLooking()
	{
		Vector3 accelerationVector = player.transform.TransformDirection(playerLookingDirection.transform.forward) * 0.05f;
		playerController.Move(accelerationVector);

	}

	public void moveWhereHandsLooking()
	{
		//Debug.Log(LeftController.transform.up);
		Vector3 averagedControllerDirection = (LeftController.transform.up + RightController.transform.up) / 2;
		Vector3 accelerationVector = player.transform.TransformDirection(averagedControllerDirection) * 0.05f;

		

		if (averagedControllerDirection.x > 0.0f) {
			playerController.Move(accelerationVector);
		}
	}

	public void moveDown()
	{
		Vector3 moveDirection = Vector3.zero;
		moveDirection.y -= 0.01f;
		playerController.Move(moveDirection);
	}
}