using UnityEngine;
using System.Collections;

public class Gravity: MonoBehaviour{

	static public int N = 3;
	public Vector3[] positions = new Vector3[N];
	public Vector3[] velocities = new Vector3[N];
	public float[] masses = new float[N];
	public GameObject[] bodies = new GameObject[N];
	float AU = 149597870700f; //astronomical unit, distance from earth to sun
	float SM = 1.98855E30f; //Solar mass, mass of the sun
	float G; //universal gravitational constant, adjusted for using AU instead of meters and solar mass instead of kg
	float deltaT = 3600f; //time that passes in each increment, smaller means better calculations
	int clockmax = 200; //total increments the program will run
	public int scale = 1000; //adjust scale of everything if they're too small for camera

	//initialize the state; this can be done in the editor but doing it in code allows arithmetic
	void Start()
	{
		bodies = GameObject.FindGameObjectsWithTag ("MassBody");
		G = (6.67384E-11f / Mathf.Pow(AU,3)) * SM; //initialize G (the mathf.pow is an exponent function... sadly, c# doesn't have a built in power operator
		Debug.Log (G);
		//Sun
		GameObject sun = bodies[0]; //selects first item in bodies array as sun, second will be earth (or anything), so on
		sun.name = "Sun";
		sun.transform.localScale = new Vector3 (1, 1, 1) * (696342000f * scale) / AU; //set size
		positions [0] = Vector3.zero; //set location (origin)
		velocities [0] = Vector3.zero; //set velocity; since we are working with velocities relative to the sun, we treat it as unmoving
		masses [0] = 1; //Sun is 1 solar mass
		bodies [0] = sun; //make sure the variable reference is placed back into the array

		//Earth
		GameObject earth =  bodies[1];
		earth.name = "Earth"; 
		earth.transform.localScale = new Vector3 (1, 1, 1) * (6371000f * scale) / AU; 
		positions [1] = new Vector3(1, 0, 0); //start earth off 1 AU along the x axis
		velocities [1] = new Vector3 (0, 0, 1) * 29800 / AU; //earth velocity, in AU/s
		masses [1] = 5.97219E24f/SM; //mass of earth in solar masses
		bodies [1] = earth;
		Debug.Log (masses [1]);
		//Moon
		GameObject moon = bodies[2];
		moon.name = "Moon";
		moon.transform.localScale = new Vector3 (1, 1, 1) * (1737500f * scale) / AU;
		positions [2] = new Vector3 (1, 0, 0) * (1f + 405500000 / AU); //moon starts off colinear to sun and earth (lunar eclipse) at apogee
		velocities [2] = new Vector3 (0, Mathf.Sin (5.1f*Mathf.Deg2Rad), Mathf.Cos (5.1f*Mathf.Deg2Rad)) * 964 / AU
								+ new Vector3 (0, 0, 1) * 29800 / AU; //moon's orbit has a 5.1 degree tilt with respect to earth/sun orbital plane, 
																	  //AND you must also add the Earth's velocity because the moon is following it 
		masses [2] = 7.34767309E22f/SM;
		bodies [2] = moon;
		Debug.Log (masses [2]);
	}

	void ForceCalc()
	{
		for (int i = 0; i<N; i++)
		{
			bodies[i].transform.position = positions[i]*scale/10; //Update each body's position
		}

		for (int i=0; i<N; i++)
		{
			Vector3 accel = Vector3.zero; //prepare an initial acceleration variable to modify as the loop cycles
			for (int j=0; j<N; j++)
			{
				if (j != i)
				{
					Vector3 direction = positions[j] - positions[i]; //Find the vector connecting the two masses
					float Rsq = Vector3.SqrMagnitude(direction); //Calculate square distance between masses
					float a = G * masses[j]/Rsq; //Calculate acceleration due to gravity
					accel += a*direction.normalized; //Turn acceleration into a vector, pointing OPPOSITE the direction vector, because gravity pulls. Add it to the total accel
				}
			}
			velocities[i] += deltaT * accel; //Update velocity using v(t+dt) = v(t) + a*dt
		}
		for (int i=1; i<N; i++)
		{
			positions[i] += deltaT * velocities[i]; //Update positions using x(t+dt) = x(t) + v*dt
		}
	}

	void FixedUpdate()
	{
		ForceCalc();
	}


}