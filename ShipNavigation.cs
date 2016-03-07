using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShipNavigation : MonoBehaviour {

	float Mdot;
	float Vexit;
	float deltaT;
	float AU;
	float SM;
	float originalMass;
	public float mass;
	float speed;
	Vector3 currentVelocity;
	public float currentFuelUsage;
	public float fuelUsage;
	public Vector3 destination;
	public Vector3 currentPosition;
	public Vector3 initialPosition;
	Vector3 totalDistance;
	bool targetReached;
	public GravityV2 gravity;
	public int currentTime;
	float G;
	public bool activated = false;
	Body[] bodies;
	int N;
	public InputField[] target;
	public bool efficient;
	
	void Start () 
	{
		Mdot = 471.7833f; // (kg/s) value calculated from simplified thrust equation and specific Impulse
		Vexit = 4.43e3f; // (m/s) value from specific Impulse of Space Shuttle Main Engine (SSME)
		originalMass = 80019.5908f; // (kg) mass taken from space shuttle Atlantis
		mass = originalMass;
		speed = 1e-6f; //ship's speed, converted from m/s to AU/s
		currentVelocity = Vector3.zero;
		currentPosition = initialPosition;
		fuelUsage = 0;
		efficient = true;
	}
	
	void FixedUpdate () 
	{
		currentTime = gravity.currentTime;
		if (activated)
		{
			ApplyForce();
			Debug.Log(currentVelocity.x);
			CancelGravity();
			Debug.Log(currentVelocity.x);
		}

	}

	void ApplyForce()
	{
		if (!targetReached)
		{
			Vector3 distance = destination - currentPosition;
			Vector3 direction = distance.normalized;
			currentPosition += speed * deltaT * direction;

			if (distance.sqrMagnitude < .01)
			{
				targetReached = true;
				gravity.isPaused = true;
				activated = false;
			}
		}
	}

	public void Activate()
	{
		activated = !activated;
		if (N == 0)
		{
			bodies = gravity.Body;
			N = gravity.N;
		}
		G = gravity.G;
		deltaT = gravity.deltaT;
		AU = gravity.AU;
		SM = gravity.SM;
		totalDistance = (destination - initialPosition) * AU / 100;
	}

	void CancelGravity()
	{
		Vector3 totalThrust = Vector3.zero;
		for (int i=0; i<N; i++)
		{
			Vector3 R = (bodies[i].positionLog[currentTime] - currentPosition/100)*AU;
			float thrust = mass*G*bodies[i].mass*SM/R.sqrMagnitude; //thrust with units 
																	//kg*m/s^2
			totalThrust += thrust*R.normalized; //collect all the thrusts and pile them 
												//into the total thrust vector
		}
		if(!efficient)
		{
			fuelUsage += (totalThrust.magnitude * deltaT) / (Vexit); // fuel usage over 
																	//time in kg = thrust 
																	//divided by specific 
																	//impulse velocity
			mass = originalMass - fuelUsage;
		}
		else
		{
			Vector3 T = (destination - currentPosition).normalized;
			Vector3 projection = Vector3.Dot(totalThrust,T)*T; //the vector projection of 
															   //gravitational force onto 
															   //the target direction
			Vector3 cancel = projection - totalThrust;
			fuelUsage += (cancel.magnitude * deltaT) / (Vexit); // this time we only use 
																//fuel to cancel gravity 
																//that slows us down.
			mass = originalMass - fuelUsage;
			Vector3 accel = projection/mass;
			currentVelocity += accel*deltaT*100/AU;//convert it from m to our units
			currentPosition += currentVelocity*deltaT;
			transform.position = currentPosition;
		}
	}

	public void UpdateDestination()
	{
		float x = float.Parse (target [0].text);
		float y = float.Parse (target [1].text);
		float z = float.Parse (target [2].text);
		destination = new Vector3 (x, y, z);
	}

	public void EfficiencyToggle()
	{
		efficient = !efficient;
	}
}
