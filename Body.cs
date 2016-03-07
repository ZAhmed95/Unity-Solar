using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Body : MonoBehaviour {

	public int clockmax; //maximum simulation ticks

	public Vector3[] positionLog; //log of body's position at any timestep

	public Vector3[] accelLog; //log of body's acceleration at any timestep

	public Vector3[] velocityLog; //log of body's velocity at any timestep

	public Vector3 initialPosition; //initial position of body (m)

	public Vector3 initialVelocity; //initial velocity of body (m/s)

	public float mass; //mass of body (kg)

	public float radius; //note: this radius is 3002.27748x the actual radius in AU
	GravityV2 gravity;
	Vector3 identity;
	public float sizeLimit;

	void Awake()
	{
		clockmax = 100000;
		GameObject main = GameObject.FindGameObjectWithTag ("MainCamera");
		gravity = main.GetComponent<GravityV2> ();
		positionLog = new Vector3[clockmax+1];
		accelLog = new Vector3[clockmax+1];
		velocityLog = new Vector3[clockmax+1];
		positionLog [0] = initialPosition;
		velocityLog [0] = initialVelocity;
		identity = new Vector3 (1, 1, 1);
		transform.localScale = identity * radius;
		sizeLimit = 20f;
		gameObject.renderer.material.color = new Vector4 (1, 1, 1, 1);
	}

	void Update()
	{
		float scaling = radius * gravity.sizeScale;
		if (scaling > 0 && scaling < sizeLimit)
			transform.localScale = identity * scaling;
		else
			transform.localScale = identity * sizeLimit;
	}
}
