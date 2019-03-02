using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

public class RotatorInput : MonoBehaviour {

	public LabManager lab;

	public float rotationSpeed;
	public GameObject rotator;

	private bool thinking;


	// Use this for initialization
	void Start () {
		thinking = true;
	}

	// Update is called once per frame
	void Update () {
		//OVRInput.Update ();
		if (thinking) {
			if (Input.GetKeyDown ("up") || GvrControllerInput.IsTouching) {		// Touchpad pressed
				float input = GvrControllerInput.TouchPos.x;//OVRInput.Get (OVRInput.Axis2D.PrimaryTouchpad);
				if (input < 0.5) {
					input = (1 - input) * -1;
				}
				transform.Rotate (Vector3.up * rotationSpeed * Time.deltaTime * input);
			} 
			if (Input.GetKeyDown ("down") || GvrControllerInput.AppButtonDown ) {// else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) == true) {
				thinking = false;
			}
		} else {
			if (Input.GetKeyDown ("up") || GvrControllerInput.IsTouching) {		// Touchpad pressed
				thinking = true;
			} else if (Input.GetKeyDown ("down") || GvrControllerInput.AppButtonDown ) {
				thinking = true;
				lab.setInput (transform.localEulerAngles.y);
				Debug.Log (transform.localEulerAngles.y);
				transform.localEulerAngles = Vector3.zero;
				lab.notWaitingAnymore ();
			}
		}
	}
}
