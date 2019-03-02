using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

public class RotatorInput : MonoBehaviour {

	public LabManager lab;

	public float rotationSpeed;
	public Text uiText;
	public Text userText;
	public bool active;

	private bool thinking;


	// Use this for initialization
	void Start () {
		uiText.text = "";
		userText.text = "";
	}

	// Update is called once per frame
	void Update () {
		//OVRInput.Update ();
		if (active) { 
			if (thinking) {
				if (Input.GetKeyDown ("up") ) {		// Touchpad pressed
					Vector2 input = new Vector2(0,0);//OVRInput.Get (OVRInput.Axis2D.PrimaryTouchpad);
					transform.Rotate (Vector3.up * rotationSpeed * Time.deltaTime * input.x);
				} else if (Input.GetKeyDown ("down") ) {// else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) == true) {
					uiText.text = transform.rotation.eulerAngles.y.ToString ();
					userText.text = "Sure?";
					thinking = false;
				}
			} else {
				if (Input.GetKeyDown ("up") ) {		// Touchpad pressed
					thinking = true;
					userText.text = "";
					uiText.text = "";
				} else if (Input.GetKeyDown ("down") ) {
					uiText.text = "conf.";
					userText.text = "";
					thinking = true;
					lab.setInput (transform.rotation.x);
				}
			}
		}
	}
}
