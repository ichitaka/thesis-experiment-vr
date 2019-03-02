using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogManager : MonoBehaviour {

	public GameObject fogSphere;

	private float density;

	// Use this for initialization
	void Start () {
		density = 0;
		//if i want to change the colour
		//Renderer fogRenderer = fogSphere.GetComponent<Renderer>();
		//fogRenderer.material.SetColor("_Color",);
		fogSphere.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	}

	#region Labfunctions
	public void createFog() {
		StartCoroutine (doFog());
	}

	public void destroyFog() {
		StartCoroutine (undoFog ());
	}

	IEnumerator doFog() {
		RenderSettings.fog = true;
		fogSphere.SetActive (true);
		while (density < 1) {
			density += 0.005f;
			RenderSettings.fogDensity = density;
			yield return null;
		}
		while (density > 0.0001f) {
			density -= 0.005f;
			RenderSettings.fogDensity = density;
			yield return null;
		}
		fogSphere.SetActive (false);
		RenderSettings.fog = false;
	}

	IEnumerator undoFog() {
		while (density > 0.0001f) {
			density -= 0.005f;
			RenderSettings.fogDensity = density;
			yield return null;
		}
		fogSphere.SetActive (false);
		RenderSettings.fog = false;
	}
	#endregion
}
