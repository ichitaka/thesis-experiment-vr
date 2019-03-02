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

	private List<string[]> oCalibrationValues = new List<string[]>();
	private int[] targetSequence;

	private int rotationIndex;


	// Use this for initialization
	void Start () {
		rotationIndex = 0;

		targetSequence = new int[] {0,1,2,3};
		reshuffle(targetSequence);

		StartCoroutine (tutorial ());
	}

	// Update is called once per frame
	void Update () {
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
		blueRotator.connect ();
		ui.text = "Drücke <b>Eingabe</b> um zu starten.";
		yield return new WaitUntil (() => Input.GetKeyDown ("up") || blueRotator.buttonDown());
		ui.text = "";
		bool notfinished = true;
			for (int j = 0; j < 3; j++) {
				string[] valuesThisRound = new string[4];
				//Textanzeige wichtiger als nur Balken
				foreach (int index in targetSequence) {
					targets [index].SetActive (true);
					if (index == 0) {
						ui.text = "Zeigen Sie nach vorne.";
					}
					if (index == 1) {
						ui.text = "Zeigen Sie nach rechts.";
					}
					if (index == 2) {
						ui.text = "Zeigen Sie nach hinten.";
					}
					if (index == 3) {
						ui.text = "Zeigen Sie nach links.";
					}
					yield return new WaitUntil (() => Input.GetKeyDown ("up") || blueRotator.buttonDown ());

					//Eingabe speichern
					valuesThisRound [index] = blueRotator.rotationInput().ToString();
					targets [index].SetActive (false);
					ui.text = "";
					yield return null;
				}
				oCalibrationValues.Add (valuesThisRound);
			}
		writeFile ();
		AsyncOperation asynchLoad = SceneManager.LoadSceneAsync (1);
		while (!asynchLoad.isDone) {
			yield return null;
		}
	}

	void writeFile() {
		string[][] output = new string[oCalibrationValues.Count][];
		for (int i = 0; i < output.Length; i++) {
			output [i] = oCalibrationValues [i];
		}

		int length = output.GetLength (0);
		string delimiter = ";";

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
		return Application.dataPath +"/CSV/"+"calibrationValues_"+subject_date_time+".csv";
		#elif UNITY_ANDROID
		return Application.persistentDataPath+"calibrationValues_"+subject_date_time+".csv";
		#endif
	}

}
