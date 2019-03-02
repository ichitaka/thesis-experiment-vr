using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

	public Text ui;
	public GameObject fog;

	// Use this for initialization
	void Start () {
		StartCoroutine (tutorial ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator tutorial() {
		ui.text = "Drücke <b>Eingabe</b> um den Versuch zu starten.";
		yield return new WaitUntil (() => Input.GetKeyDown ("up") || GvrControllerInput.AppButtonDown);
		RenderSettings.fog = true;
		fog.SetActive (true);
		Renderer fogRen = fog.GetComponent<Renderer> ();
		float density = 0;
		Color fogCol = fogRen.material.color;
		fogRen.material.color = new Color (fogCol.r, fogCol.b, fogCol.g, density);
		while (density < 1) {
			density += 0.005f;
			RenderSettings.fogDensity = density;
			fogRen.material.color = new Color (fogCol.r, fogCol.b, fogCol.g, density);
			yield return null;
		}
		AsyncOperation asynchLoad = SceneManager.LoadSceneAsync (1);
		while (!asynchLoad.isDone) {
			yield return null;
		}
	}
			
}
