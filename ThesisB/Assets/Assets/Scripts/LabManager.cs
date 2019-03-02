using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;

public class LabManager : MonoBehaviour {

	private List<Trial> trials;
	private string oCurrentTarget;
	private int currentOri;

	public GameObject subject;
	public GameObject rotator;
	public GameObject viewblocker;
	public GameObject fogSphere;
	public Text ui;

	private bool waiting;
	private Camera subjectCamera;

	private float density;

	private List<string[]> oRowData = new List<string[]>();
	private string oCurrentLocation;
	private float oInputData;


	// Use this for initialization
	void Start () {
		subjectCamera = subject.GetComponentsInChildren<Camera> ()[0];
		viewblocker.SetActive (false);
		oCurrentLocation = "Init";
		waiting = false;
		rotator.SetActive(false);
		trials = new List<Trial>();
		trials.Add (new Trial ("Eschenheimer Turmwarte"));
		trials.Add (new Trial ("Brunnen Römerberg"));
		trials.Add (new Trial ("Cafe Hauptwache"));
		trials.Add (new Trial ("Euro-Skulptur Willy-Brandt-Platz"));
		for (int i = 0; i < trials.Count; i++) {
			Trial temp = trials[i];
			int randomIndex = UnityEngine.Random.Range(i, trials.Count);
			trials[i] = trials[randomIndex];
			trials[randomIndex] = temp;
		}
		for (int i = 0; i < trials.Count; i++) {
			trials [i].reshuffle<string> (trials [i].targets);
		}


		oCurrentTarget = "Init";
		currentOri = 1000; //instead of null we use 1000 as a placeholder value
		oInputData = -500;
		ui.text = "";


		string[] rowDataTemp = new string[8];
		rowDataTemp [0] = "Where";
		rowDataTemp [1] = "Euler1";
		rowDataTemp [2] = "Euler2";
		rowDataTemp [3] = "Euler3";
		rowDataTemp [4] = "Time";
		rowDataTemp [5] = "Input";
		rowDataTemp [6] = "Target";
		rowDataTemp [7] = "Ori";
		oRowData.Add (rowDataTemp);

		//fogSphere.SetActive (true);
		density = 1;
		RenderSettings.fog = true;
		RenderSettings.fogDensity = density;

		StartCoroutine (targetLoop ());
	}
	
	// Update is called once per frame
	void Update () {
		string[] rowDataTemp = new string[8];
		rowDataTemp [0] = oCurrentLocation;
		rowDataTemp [1] = subjectCamera.transform.eulerAngles.x.ToString();
		rowDataTemp [2] = subjectCamera.transform.eulerAngles.y.ToString();
		rowDataTemp [3] = subjectCamera.transform.eulerAngles.z.ToString();
		rowDataTemp [4] = Time.deltaTime.ToString();
		//TODO where to get input data
		if (oInputData != -500) {
			rowDataTemp [5] = oInputData.ToString();
			oInputData = -500;
		} else {
			rowDataTemp [5] = "null";
		}
		rowDataTemp [6] = oCurrentTarget;
		rowDataTemp [7] = Convert.ToString (currentOri);
		oRowData.Add (rowDataTemp);
	}

	#region Labfunctions

	IEnumerator targetLoop() {
		for (int l = 0; l < 2; l++) {
			for (int i = 0; i < trials.Count; i++) {
				//teleport here
				ui.text = "";
				destroyFog (trials[i].place);
				yield return new WaitUntil (() => !waiting);
				ui.text = "Schauen Sie sich um. Mit <b>Eingabe</b> starten Sie die Aufgabe.";
				oCurrentTarget = "OriPhase";
				currentOri = 1000; //1000 is placeholder value for orientation phase
				yield return new WaitUntil (() => Input.GetKeyDown ("up") || GvrControllerInput.AppButtonDown);
				subjectCamera.farClipPlane = 60f;
				if (trials [i].place == "Eschenheimer Turmwarte") {
					subjectCamera.farClipPlane = 110f;
				}
				for (int j = 0; j < 5; j++) {
					waiting = true;
					oCurrentTarget = trials[i].targets [j+l*5];
					currentOri = trials[i].orientations [j+l*5];
					subject.transform.rotation = Quaternion.Euler (new Vector3(0,currentOri,0));
					//orient subject if every target			
					rotator.SetActive (true);
					viewblocker.SetActive (true);
					//viewblocker
					//ui changes
					ui.text = "Bitte zeige nach <b>" + oCurrentTarget + "</b>";
					//get input by activating input class
					yield return new WaitUntil (() => !waiting);  //test if i can use this to jump to next phase
					rotator.SetActive (false);
					viewblocker.SetActive (false);
				}
				ui.text = "";
				subjectCamera.farClipPlane = 300f;
				createFog ();
				yield return new WaitUntil (() => !waiting);
			}
		}
		AsyncOperation asynchLoad = SceneManager.LoadSceneAsync (1);
		writeFile ();
		while (!asynchLoad.isDone) {
			yield return null;
		}
	}


	//This function takes a name of a location and moves subject accordingly
	void moveSubject (String name) {
		// set location for data
		oCurrentLocation = name;
		if (name == "Eschenheimer Turmwarte") {
			subject.transform.position = new Vector3 (42f, 1.8f, -18f);
			return;
		}		
		if (name == "Brunnen Römerberg") {
			subject.transform.position = new Vector3 (240f, 1.8f, -563f);
			return;
		}
		if (name == "Euro-Skulptur Willy-Brandt-Platz") {
			subject.transform.position = new Vector3 (-488f, 1.8f, -625f);
			return;
		}
		if (name == "Haupteingang C&A Konstablerwache") {
			subject.transform.position = new Vector3 (418f, 1.8f, -246f);
			return;
		}
		if (name == "Cafe Hauptwache") {
			subject.transform.position = new Vector3 (-2f, 1.8f, -300f);
			return;
		}
	}

	void writeFile() {
		string[][] output = new string[oRowData.Count][];
		for (int i = 0; i < output.Length; i++) {
			output [i] = oRowData [i];
		}

		int length = output.GetLength (0);
		string delimiter = ",";

		StringBuilder sb = new StringBuilder ();

		for (int index = 0; index < length; index++) {
			sb.AppendLine (string.Join (delimiter, output [index]));
		}

		string filePath = getPath ();

		StreamWriter outStream = System.IO.File.CreateText (filePath);
		outStream.WriteLine (sb);
		outStream.Close ();
	}

	// Following method is used to retrive the relative path as device platform
	private string getPath(){
		System.DateTime now = System.DateTime.Now;
		string subject_date_time =  now.Day.ToString() + "-" + now.Month.ToString() + "-" + now.Hour.ToString()+now.Minute.ToString();
		#if UNITY_EDITOR
		return Application.dataPath +"/CSV/"+"Saved_data_B_"+subject_date_time+".csv";
		#elif UNITY_ANDROID
		return Application.persistentDataPath+"Saved_data_B_"+subject_date_time+".csv";
		#elif UNITY_IPHONE
		return Application.persistentDataPath+"/"+"Saved_data_" + System.DateTime.UtcNow.ToString() + ".csv";
		#else
		return Application.dataPath +"/"+"Saved_data_" + System.DateTime.UtcNow.ToString() + ".csv";
		#endif
	}

	public void setInput(float input) {
		oInputData = input;
	}

	public void destroyFog(string name) {
		oCurrentTarget = "Fog";
		StartCoroutine (dontFog (name));
	}

	public void createFog() {
		oCurrentTarget = "Fog";
		StartCoroutine (doFog());
	}

	public void notWaitingAnymore() {
		waiting = false;
	}

	IEnumerator doFog() {
		waiting = true;
		RenderSettings.fog = true;
		while (density < 1) {
			density += 0.015f;
			RenderSettings.fogDensity = density;
			yield return null;
		}
		waiting = false;
	}

	IEnumerator dontFog(string name) {
		waiting = true;
		moveSubject (name);
		while (density > 0.0001f) {
			density -= 0.015f;
			RenderSettings.fogDensity = density;
			yield return null;
		}
		RenderSettings.fog = false;
		waiting = false;
	}
	
	#endregion
}
