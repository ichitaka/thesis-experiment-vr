using UnityEngine;
using System.Collections.Generic;

public class Trial
{
	public string place;
	public List<targetPair> targets; 

	public Trial() {
		place = "Test";
		targets = new List<targetPair> ();
		targetPair target1 = new targetPair("Target1", new Vector3(0,45,0));
		targetPair target2 = new targetPair("Target2", Vector3.zero);
		targetPair target3 = new targetPair("Target3", new Vector3(0, 85, 0));
		targets.Add (target1);
		targets.Add (target2);
		targets.Add (target3);
	}
}

public struct targetPair{
	public string target;
	public Vector3 orientation;

	public targetPair (string t, Vector3 o) {
		target = t;
		orientation = o;
	}
}

