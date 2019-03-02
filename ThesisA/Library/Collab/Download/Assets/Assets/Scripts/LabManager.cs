using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;

public class LabManager : MonoBehaviour {

	public List<string> position2;
	private List<Trial> trials;
	private Trial currentTrial;
	private string currentTarget;
	private string currentOri;

	public GameObject subject;
	public GameObject rotator;
	public ReadBlueData blueRotator;
	public GameObject viewblocker;
	public GameObject fogSphere;
	public ReadBlueData demo;
	public Text ui;


	private Hashtable positions = new Hashtable();

	private int phase;
	private bool waiting;

	private float density;

	private List<string[]> rowData = new List<string[]>();
	private string currentLocation;
	private float inputData;

	// Use this for initialization
	void Start () {
		viewblocker.SetActive (false);
		phase = 0;
		currentLocation = "Init";
		waiting = false;
		rotator.SetActive(false);
		trials = new List<Trial>();
		trials.Add (new Trial ());
		trials.Add (new Trial ());
		currentTrial = new Trial ();
		currentTarget = "";
		currentOri = "";
		ui.text = "";

		positions.Add ("Test", new Vector3 (0f, 1.8f, 0f));
		positions.Add ("Test2", new Vector3 (1f, 1.8f, 5f));
		positions.Add ("Test3", new Vector3 (3f, 1.8f, -3f));

		string[] rowDataTemp = new string[8];
		rowDataTemp [0] = "Where";
		rowDataTemp [1] = "Euler1";
		rowDataTemp [2] = "Euler2";
		rowDataTemp [3] = "Euler3";
		rowDataTemp [4] = "Time";
		rowDataTemp [5] = "Input";
		rowDataTemp [6] = "Target";
		rowDataTemp [7] = "Ori";
		rowData.Add (rowDataTemp);

		fogSphere.SetActive (true);
		density = 1;
		RenderSettings.fog = true;
		RenderSettings.fogDensity = density;

		blueRotator.connect ();
		StartCoroutine (targetLoop ());
	}
	
	// Update is called once per frame
	void Update () {
		string[] rowDataTemp = new string[8];
		//OVRInput.Update ();
		//if (Input.GetKeyDown ("up") || GvrControllerInput.AppButtonDown ) {//|| OVRInput.Get (OVRInput.Button.PrimaryIndexTrigger) == true) {
		//	if (!waiting) {
		//		doPhase ();
		//	}
		//}
		rowDataTemp [0] = currentLocation;
		rowDataTemp [1] = subject.transform.eulerAngles.x.ToString();
		rowDataTemp [2] = subject.transform.eulerAngles.y.ToString();
		rowDataTemp [3] = subject.transform.eulerAngles.z.ToString();
		rowDataTemp [4] = Time.deltaTime.ToString();
		//TODO where to get input data
		if (inputData != -500) {
			rowDataTemp [5] = inputData.ToString();
			inputData = -500;
		} else {
			rowDataTemp [5] = "null";
		}
		rowDataTemp [6] = currentTarget;
		rowDataTemp [7] = currentOri;
		rowData.Add (rowDataTemp);
	}

	#region Labfunctions
	//This function decides what comes next
	void doPhase() {
		if (phase == 0) {
			destroyFog ();
			phase++;
		} else if (phase == 1) {
			StartCoroutine(targetLoop());
			phase++;
		} else if (phase == 2) {
			writeFile ();
			createFog ();
			phase = 0;
		}
	}

	IEnumerator targetLoop() {
		for(int i = 0; i < trials.Count; i++) {
			destroyFog ();
			yield return new WaitUntil(() => !waiting);
			ui.text = "Schauen Sie sich um. Mit <b>Eingabe</b> starten Sie die Aufgabe.";
			yield return new WaitUntil (() => Input.GetKeyDown ("up") || GvrControllerInput.AppButtonDown);//|| blueRotator.buttonDown());
			for(int j = 0; j < currentTrial.targets.Count; j++) {
				waiting = true;
				targetPair target = currentTrial.targets [j];
				currentTarget = target.target;
				currentOri = target.orientation.y.ToString ();
				subject.transform.rotation = Quaternion.Euler (target.orientation);
				//orient subject if every target			
				rotator.SetActive(true);
				blueRotator.measureInput ();
				viewblocker.SetActive (true);
				//viewblocker
				//ui changes
				ui.text = "Bitte zeige nach <b>" + target.target + "</b>";
				//get input by activating input class
				yield return new WaitUntil(() => !waiting);  //test if i can use this to jump to next phase
				rotator.SetActive(false);
				viewblocker.SetActive (false);
			}
			ui.text = "";
			createFog ();
			yield return new WaitUntil(() => !waiting);
		}
		AsyncOperation asynchLoad = SceneManager.LoadSceneAsync (2);
		while (!asynchLoad.isDone) {
			yield return null;
		}
	}


	//This function takes a name of a location and moves subject accordingly
	void moveSubject (String name) {
		subject.transform.position = (Vector3)positions[name];
		//set location for data
		currentLocation = name;
	}

	void writeFile() {
		string[][] output = new string[rowData.Count][];
		for (int i = 0; i < output.Length; i++) {
			output [i] = rowData [i];
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
		#if UNITY_EDITOR
		return Application.dataPath +"/CSV/"+"Saved_data.csv";
		#elif UNITY_ANDROID
		return Application.persistentDataPath+"Saved_data.csv";
		#elif UNITY_IPHONE
		return Application.persistentDataPath+"/"+"Saved_data.csv";
		#else
		return Application.dataPath +"/"+"Saved_data.csv";
		#endif
	}

	public void setInput(float input) {
		inputData = input;
	}

	public void destroyFog() {
		StartCoroutine (dontFog ());
	}

	public void createFog() {
		StartCoroutine (doFog());
	}

	public void notWaitingAnymore() {
		waiting = false;
	}

	IEnumerator doFog() {
		waiting = true;
		RenderSettings.fog = true;
		fogSphere.SetActive (true);
		while (density < 1) {
			density += 0.005f;
			RenderSettings.fogDensity = density;
			yield return null;
		}
		waiting = false;
	}

	IEnumerator dontFog() {
		waiting = true;
		moveSubject (position2 [0]);
		position2.RemoveAt (0);
		while (density > 0.0001f) {
			density -= 0.005f;
			RenderSettings.fogDensity = density;
			yield return null;
		}
		fogSphere.SetActive (false);
		RenderSettings.fog = false;
		waiting = false;
	}
	
	#endregion
}
