using UnityEngine;
using System.Collections.Generic;
using System;

public class Trial
{
	public string place;
	public string[] targets; 

	public int[] orientations;

	public Trial(string name) {
		place = name;
		targets = new string[]{
			"Haupteingang Alte Oper",
			"Euro-Skulptur Willy-Brandt-Platz",
			"Brunnen Römerberg",
			"Eschenheimer Turmwarte",
			"Brockhaus Brunnen Konstablerwache",
			"Gutenberg Denkmal / Roßmarkt",
			"Haupteingang C&A Konstablerwache",
			"Cafe Hauptwache",
			"Aufgang Eiserner Steg Frankfurter Seite",
			"Springbrunnen Kaiserplatz / Eingang Commerzbank Tower",
			"Eingang Portikus"};
		//"Haupteingang Stadtzoo" "Haupteingang Städel"
		int indexOfInit = Array.IndexOf (targets, place);
		List<string> tmp = new List<string> (targets);
		tmp.RemoveAt (indexOfInit);
		targets = tmp.ToArray ();


		orientations = new int[] { 0, 36, 72, 108, 144, 180, 216, 252, 288, 324};     
		reshuffle (orientations);
	}

	public void reshuffle<T>(T[] array)
	{
		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
		for (int t = 0; t < array.Length; t++ )
		{
			T tmp = array[t];
			int r = UnityEngine.Random.Range(t, array.Length);
			array[t] = array[r];
			array[r] = tmp;
		}
	}
}

