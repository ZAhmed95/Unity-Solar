using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SaveData : MonoBehaviour {

	GameObject[] bodies;
	public static Body[] body;
	public static Vector3[,] positionLogs;

	public void UpdateSave()
	{
		bodies = GameObject.FindGameObjectsWithTag ("MassBody");
		body = new Body[bodies.Length];
		int clockmax = body [0].clockmax;
		for (int i=0; i<bodies.Length; i++)
		{
			body[i] = bodies[i].GetComponent<Body>();
			for (int j=0; j<clockmax; j++)
			{
				positionLogs[i,j] = body[i].positionLog[j];
			}
		}

	}

	public void UpdateLoad()
	{
		for (int i=0; i<body.Length; i++)
		{
		
		}
	}
}
