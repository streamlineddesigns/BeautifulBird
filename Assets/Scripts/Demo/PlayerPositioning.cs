using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositioning : MonoBehaviour {
	public GameObject anchor;
	private Vector3 offset;
	private Vector3 targetPos;
	// Use this for initialization
	void Start () {
		offset = transform.position - anchor.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		targetPos = anchor.transform.position + offset;
		transform.position = targetPos;
	}
}
