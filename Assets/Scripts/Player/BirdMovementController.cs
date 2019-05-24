using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovementController : MonoBehaviour {
	public OVRPlayerController OVRController;
	public GameObject player;
	protected CharacterController playerController;

	// Use this for initialization
	void Start () {
		playerController = player.GetComponent<CharacterController>();
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

			moveForward();

		} else if (playerController.isGrounded) {

			if (! OVRController.EnableLinearMovement) {
				OVRController.EnableLinearMovement = true;
			}

		}
	}

	public void moveUp()
	{
		OVRController.Jump();
	}

	public void moveForward()
	{
		Vector3 accelerationVector = player.transform.TransformDirection(Vector3.forward) * 0.05f;
		playerController.Move(accelerationVector);

	}

	public void moveDown()
	{
		Vector3 moveDirection = Vector3.zero;
		moveDirection.y -= 0.01f;
		playerController.Move(moveDirection);
	}
}