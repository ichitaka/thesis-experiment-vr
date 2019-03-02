using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndManager : MonoBehaviour {

	public Text ui;
	public GameObject fog;

	// Use this for initialization
	void Start () {
		StartCoroutine (end ());
	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator end() {
		ui.text = "Vielen Dank für ihre Teilnahme an diesem Versuch. Sie können das Headset nun abnehmen.";
		RenderSettings.fog = true;
		fog.SetActive (true);
		Renderer fogRen = fog.GetComponent<Renderer> ();
		float density = 1;
		Color fogCol = fogRen.material.color;
		fogRen.material.color = new Color (fogCol.r, fogCol.b, fogCol.g, density);
		while (density > 0.0001f) {
			density -= 0.015f;
			RenderSettings.fogDensity = density;
			fogRen.material.color = new Color (fogCol.r, fogCol.b, fogCol.g, density);
			yield return null;
		}
		RenderSettings.fog = false;
		fog.SetActive (false);
	}

}
