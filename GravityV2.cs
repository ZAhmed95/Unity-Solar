using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GravityV2 : MonoBehaviour {

	public int N;
	public Body[] Body;
	public Vector3[] positions;
	public Vector3[] velocities;
	public float[] masses;
	public GameObject[] bodies;
	Vector3[,] forces; //2D array of V3s that store the acceleration of each body due to every other body (i.e. "forces[1][2]" stores the acceleration of body 1 due to body 2)
	public float AU = 149597870700f; //astronomical unit, distance from earth to sun
	public float SM = 1.98855E30f; //Solar mass, mass of the sun
	public float G; //universal gravitational constant, adjusted for using AU instead of meters and solar mass instead of kg
	public float deltaT; //time that passes in each increment, smaller means better calculations
	int clockmax; //total increments the program will run
	int clock = 0; //step count of simulation
	public int scale; //adjust scale of everything if they're too small for camera
	public float sizeScale;
	public bool isPaused = true;
	public int currentTime = 0;
	public Text text;

	//initialize the state; this can be done in the editor but doing it in code allows arithmetic
	void Start()
	{
		bodies = GameObject.FindGameObjectsWithTag ("MassBody");
		N = bodies.Length;
		Body = new Body[N];
		positions = new Vector3[N];
		velocities = new Vector3[N];
		masses = new float[N];
		forces = new Vector3[N,N];
		for (int i=0; i<N; i++)
		{
			Body[i] = (Body)bodies[i].GetComponent("Body");
			positions[i] = Body[i].positionLog[0];
			velocities[i] = Body[i].velocityLog[0];
			masses[i] = Body[i].mass;
		}

		PlaySimulation();
		clockmax = Body [0].clockmax;
		for (int i=0;i<N;i++)

		G = (6.67384E-11f / Mathf.Pow(AU,3)) * SM; //initialize G (the mathf.pow is an exponent function... sadly, C# doesn't have a built in power operator
		sizeScale = 50;
	}
	
	void ForceCalc()
	{
		if (clock < clockmax)//iterates until clockmax is reached; 
							 //basically same function as for loop
		{ 
			clock++;
			for (int i=0; i<N; i++)
			{
				Vector3 accel = Vector3.zero; //declare a variable 
						//to store the total acceleration of object i
				for (int j=0; j<N; j++) //all forces acting on i 
								//due to j calculated in this loop
				{
					if (j == i) //check to make sure we don't 
								//calculate i's force on itself 
					{
						forces[i,j] = Vector3.zero;
					}

					else
					{
						Vector3 direction = positions[j] - positions[i]; 
						//Find the vector connecting the two masses
						float Rsq = Vector3.SqrMagnitude(direction); 
						//Calculate square distance between masses
						float a = G * masses[j]/Rsq;
						//Calculate acceleration due to gravity
						forces[i,j] = a*direction.normalized; 
						//store acceleration of body i due to body j in 
						//the 2D array
					}
				}

				for (int a=0; a<N; a++)
				{
					accel += forces[i,a];
				}
				Body[i].accelLog[clock] = accel; 
				//log i's current acceleration in accelLog
		
				velocities[i] += deltaT * accel; 
				//Update velocity using v(t+dt) = v(t) + a*dt
				Body[i].velocityLog[clock] = velocities[i]; 
				//log i's current velocity in velocityLog
			}
			for (int i=0; i<N; i++)
			{
				positions[i] += deltaT * velocities[i]; 
				//Update positions using x(t+dt) = x(t) + v*dt
				Body[i].positionLog[clock] = positions[i]; 
				//log i's current position in positionLog,
				//this can later be used to play, pause, rewind 
				//animation
			}
		}
	}
	
	void FixedUpdate()
	{
		ForceCalc();

		if (!isPaused)
		{
			PlaySimulation();
			text.text = "Clock = " + clock;
		}
	}

	public void TogglePause()
	{
		isPaused = !isPaused;
	}

	void PlaySimulation()
	{
		for (int i=0; i<N; i++)
		{

			bodies[i].transform.position = Body[i].positionLog[currentTime]*scale;
		}
		currentTime++;

		if (currentTime > clockmax)
		{
			isPaused = true;
			currentTime = 0;
		}
	}

}
