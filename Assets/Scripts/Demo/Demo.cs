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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//If both hand grips are being pressed
		if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0.0f &&
			OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0.0f) {

				if (DOMesh.material != onMaterial) {
					DOMesh.material = onMaterial;
				}

				setDBOrientation();

		} else {
			if (DOMesh.material != offMaterial) {
					DOMesh.material = offMaterial;
				}
		}
	}

	protected void setDBOrientation() 
	{
		pitchValue = getCalculatedPitch();
		yawValue = getCalculatedYaw();
		rollValue = getCalculatedRoll();
		DO.transform.rotation = Quaternion.Euler(pitchValue * -1.0f, yawValue, rollValue);
	}

	protected float getCalculatedPitch()
	{
		float calculatedPitch = (RC.transform.localEulerAngles.z + (LC.transform.localEulerAngles.z * -1.0f)) / 2.0f;
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