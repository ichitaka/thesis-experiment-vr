using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Xml.Linq;

public class parseTrialFile : MonoBehaviour
{
	public TextAsset file; 

	// Use this for initialization
	void Start ()
	{
		string[,] grid = SplitCSVGrid(file.text);
		Debug.Log (grid[0,0]);
		Debug.Log (grid [1, 1]);
	}

	public string[,] SplitCSVGrid(string gridText) {
		string[] lines = gridText.Split("\n"[0]); 

		string[,] outputGrid = new string[8, lines.Length + 1];
		for (int i = 0; i < lines.Length; i++) {
			string[] row = SplitCSVLine (lines [i]);
			for (int j = 0; j < row.Length; j++) {
				outputGrid [j, i] = row [j];
			}
		}
		return outputGrid;

	}

	public string[] SplitCSVLine(string line) {
		string[] output = line.Split(','); 
		return output;
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

}

