using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Body : MonoBehaviour {

	public int clockmax;
	public Vector3[] positionLog;
	public Vector3[] accelLog;
	public Vector3[] velocityLog;
	public Vector3 initialPosition;
	public Vector3 initialVelocity;
	public float mass;
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
