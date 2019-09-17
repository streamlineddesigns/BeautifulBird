using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositioner : MonoBehaviour {
	public GameObject WalkingCamera;
	public GameObject FlyingCamera;
	public GameObject WalkingTarget;
	public GameObject FlyingTarget;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (WalkingCamera.transform.position != WalkingTarget.transform.position) {
			WalkingCamera.transform.position = WalkingTarget.transform.position;
		}
	}
}
