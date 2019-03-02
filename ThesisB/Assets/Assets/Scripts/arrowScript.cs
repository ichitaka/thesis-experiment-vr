namespace VRTK.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class arrowScript : MonoBehaviour {

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void SetArrowRotation(Vector2 input) {
			if(input.x >= 0.5) {
				//move right
				transform.Rotate(0,1,0);
			} else {
				//move left
				transform.Rotate(0,-1,0);
			}
		}
	}
}
