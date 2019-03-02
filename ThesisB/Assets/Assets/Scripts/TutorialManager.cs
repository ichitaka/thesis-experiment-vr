using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using System.IO;

public class TutorialManager : MonoBehaviour {

	public Text ui;

	public GameObject[] targets;

	public ReadBlueData blueRotator;

	private int[] calibrationValues;
	private int[] targetSequence;

	private int rotationIndex;


	// Use this for initialization
	void Start () {
		blueRotator.connect ();

		calibrationValues = new int[4];
		rotationIndex = 0;

		targetSequence = new int[] {0,1,2,3};
		reshuffle(targetSequence);

		StartCoroutine (tutorial ());
	}
	
	// Update is called once per frame
	void Update () {
		//saveRot (blueRotator.rotationInput ());
	}

	void reshuffle(int[] positions)
	{
		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
		for (int t = 0; t < positions.Length; t++ )
		{
			int tmp = positions[t];
			int r = UnityEngine.Random.Range(t, positions.Length);
			positions[t] = positions[r];
			positions[r] = tmp;
		}
	}

	IEnumerator tutorial() {
		ui.text = "Drücke <b>Eingabe</b> um zu starten.";
		yield return new WaitUntil (() => Input.GetKeyDown ("up") || blueRotator.buttonDown());
		ui.text = "";
		bool notfinished = true;
		//while (notfinished) {
			for (int j = 0; j < 3; j++) {
				foreach (int index in targetSequence) {
					targets [index].SetActive (true);
					if (index == 2) {
						ui.text = "Das Ziel befindet sich hinter ihnen. Gegenüber von dem Ziel vor ihnen";
						targets [0].SetActive (true);
					}
					yield return new WaitUntil (() => Input.GetKeyDown ("up") || blueRotator.buttonDown ());
//					int average = 0;
//					int sum = 0;
//					foreach (int value in lastRotationData) {
//						sum += value;
//					}
					int input = blueRotator.rotationInput();//average = sum / 10;
					if (calibrationValues [index] == 0) {
						calibrationValues [index] = input;
					} else {
						calibrationValues [index] = (calibrationValues [index] + input) / 2;
					}
					targets [index].SetActive (false);
					if (index == 2) {
						ui.text = "";
						targets [0].SetActive (false);
					}
					yield return null;
				}
			}
//			notfinished = false;
//			for (int i = 0; i < calibrationValues.Length - 1; i++) {
//				float diff = (calibrationValues [i]%102) - (calibrationValues [i + 1]%102);
//				if (diff > 16 || diff < 9) {
//					notfinished = true;
//					ui.text = "Calibration Error";
//				}
//			}
//		}
		writeFile ();
		AsyncOperation asynchLoad = SceneManager.LoadSceneAsync (1);
		while (!asynchLoad.isDone) {
			yield return null;
		}
	}

	void writeFile() {
		string[] output = new string[4];
		for (int i = 0; i < output.Length; i++) {
			output [i] = calibrationValues [i].ToString();
		}

		string delimiter = ";";

		StringBuilder sb = new StringBuilder ();

		sb.AppendLine (string.Join (delimiter, output));


		string filePath = getPath ();

		StreamWriter outStream = System.IO.File.CreateText (filePath);
		outStream.WriteLine (sb);
		outStream.Close ();
	}

	// Following method is used to retrive the relative path as device platform
	private string getPath(){
		#if UNITY_EDITOR
		return Application.dataPath +"/CSV/"+"calibrationValues.csv";
		#elif UNITY_ANDROID
		return Application.persistentDataPath+"calibrationValues.csv";
		#elif UNITY_IPHONE
		return Application.persistentDataPath+"/"+"Saved_data.csv";
		#else
		return Application.dataPath +"/"+"Saved_data.csv";
		#endif
	}

}
