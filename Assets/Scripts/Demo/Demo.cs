using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour {
	public GameObject RC;
	public GameObject LC;
	public GameObject DO;
	public MeshRenderer DOMesh;
	public GameObject DB;
	public Material onMaterial;
	public Material offMaterial;
	private float pitchValue;
	private float yawValue;
	private float rollValue;
	private Vector3 AverageEulers;

	// Use this for initialization
	void Start () {
		AverageEulers = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		//If both hand grips are being pressed
		if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0.0f &&
			OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.0f) {

				/*if (DOMesh.material != onMaterial) {

					DOMesh.material = onMaterial;

				} */

				setDBOrientation();

		} else {
			/*if (DOMesh.material != offMaterial) {
				DOMesh.material = offMaterial;
			} */
		}
	}

	protected void setDBOrientation() 
	{
		//pitchValue = getCalculatedPitch();
		//yawValue = getCalculatedYaw();
		//rollValue = getCalculatedRoll();

		/*
		//RIGHT Controller Perfect
		pitchValue = RC.transform.eulerAngles.z;
		yawValue = RC.transform.eulerAngles.y;
		rollValue = RC.transform.eulerAngles.x;

		DB.transform.eulerAngles = new Vector3(pitchValue, yawValue - 90.0f, -rollValue);
		 */

		/*
		//LEFT Controller Perfect 
		pitchValue = LC.transform.eulerAngles.z;
		yawValue = LC.transform.eulerAngles.y;
		rollValue = LC.transform.eulerAngles.x;

		DB.transform.eulerAngles = new Vector3(-pitchValue, yawValue + 90.0f, rollValue);
		
		*/

		
		//Theoretically should work but does now.. without the division for averages
		pitchValue = (RC.transform.eulerAngles.z + (LC.transform.eulerAngles.z * -1.0f));
		yawValue = (RC.transform.eulerAngles.y + LC.transform.eulerAngles.y);
		rollValue = ((RC.transform.eulerAngles.x * -1.0f) + LC.transform.eulerAngles.x);

		//DB.transform.eulerAngles = new Vector3(pitchValue, yawValue, rollValue); //Old working thing
		DB.transform.rotation = Quaternion.Lerp(DB.transform.rotation, Quaternion.Euler(pitchValue, yawValue, rollValue), 0.025f); //Testing lerp

		//New test
		//DB.transform.rotation = Quaternion.Euler(pitchValue,0,0);
		 

		/*
		//working example of both controllers
		float pitchValue1 = RC.transform.eulerAngles.z;
		float pitchValue2 = - LC.transform.eulerAngles.z;
		float pitchValue3 = pitchValue1 + pitchValue2;
		float pitchValue4 = pitchValue3 / 2;

		DB.transform.eulerAngles = new Vector3(pitchValue4, 0.0f, 0.0f);
		 */

	}

	protected float getCalculatedPitch()
	{
		//float calculatedPitch = (RC.transform.eulerAngles.z + (LC.transform.eulerAngles.z * -1.0f)) / 2.0f;
		//float calculatedPitch = (RC.transform.eulerAngles.z + (LC.transform.eulerAngles.z * -1.0f)) / 2.0f;
		//float calculatedPitch = (Mathf.Abs(RC.transform.eulerAngles.z) + Mathf.Abs(LC.transform.eulerAngles.z));
		if ((RC.transform.eulerAngles.z + (LC.transform.eulerAngles.z * -1.0f)) > AverageEulers.x) {

			AverageEulers.x += 0.1f;

		} else if ((RC.transform.eulerAngles.z + (LC.transform.eulerAngles.z * -1.0f)) < AverageEulers.y) {
			//AverageEulers.x -= 0.1f;
		}

		//float calculatedPitch = (RCEulers.x + LCEulers.x) / 2.0f;
		float calculatedPitch = AverageEulers.x;

		return calculatedPitch;
	}
	protected float getCalculatedYaw()
	{
		float calculatedYaw = (RC.transform.eulerAngles.y + LC.transform.eulerAngles.y) / 2.0f;
		return calculatedYaw;
	}
	protected float getCalculatedRoll()
	{
		float calculatedRoll = ((RC.transform.eulerAngles.x * -1.0f) + LC.transform.eulerAngles.x) / 2.0f;
		return calculatedRoll;
	}

	private float UnwrapAngle(float angle)
    {
        if(angle >=0)
            return angle;
 
        angle = -angle%360;
 
        return 360-angle;
    }

}

/*
NEEDS:

Player Controllers
Demo Object

Roll - Z Axis Rotation
Yaw - Y Axis Rotation
Pitch - X Axis Rotation

X = RED
Y = GREEN
Z = BLUE




 */