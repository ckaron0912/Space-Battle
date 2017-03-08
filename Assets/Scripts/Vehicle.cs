using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]


abstract public class Vehicle : MonoBehaviour {
	
	public float maxSpeed = 8.0f;
	public float maxForce = 3.0f;
	public float mass = 1.0f;
	public float radius = 1.0f;
	public float gravity = 20.0f;
	private int health = 100;

	public int Health {
		get { return health; }
		set { health = value;}
	}

	private bool capital = false;
	public bool Capital {

		get { return capital; }
		set { capital = value;}
	}

	//wander
	float wanderRad = 1.0f;
	float wanderDist = 5.0f;
	float wanderRand = 3.0f;
	float wanderAng = 0.2f;

	private int frames = 2;
	
	protected CharacterController characterController;
	protected Vector3 acceleration;	//change in velocity per second
	protected Vector3 velocity;		//change in position per second
	protected Vector3 dv;           //desired velocity
	public Vector3 Velocity {
		get { return velocity; }
		set { velocity = value;}
	}
	
	//Classes that extend Vehicle must override CalcSteeringForce
	abstract protected void CalcSteeringForce();
	
	virtual public void Start(){
		acceleration = Vector3.zero;
		velocity = transform.forward;
		//get component references
		characterController = gameObject.GetComponent<CharacterController> ();
	}
	
	
	// Update is called once per frame
	protected void Update () {
		CalcSteeringForce ();
		
		//update velocity
		velocity += acceleration * Time.deltaTime;
		//velocity.y = 0;	// we are staying in the x/z plane
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
		
		//orient the transform to face where we going
		if (velocity != Vector3.zero)
			transform.forward = velocity.normalized;
		
		// keep us grounded
		velocity.y -= gravity * Time.deltaTime;
		
		// the CharacterController moves us subject to physical constraints
		characterController.Move (velocity * Time.deltaTime);
		
		//reset acceleration for next cycle
		acceleration = Vector3.zero;
		
	}
	
	protected void ApplyForce (Vector3 steeringForce){
		acceleration += steeringForce/mass;
	}
	

	protected Vector3 Seek (Vector3 targetPos)
	{
		//find dv, the desired velocity
		dv = targetPos - transform.position;
		
		//make the magnitude equal to maxSpeed
		dv.Normalize();
		dv = dv * maxSpeed;

		Vector3 force = dv - Velocity;
		
		return force;
	}

	protected Vector3 Pursue(Vehicle target)
	{
		//Debug.DrawLine(transform.position, target.transform.position, Color.green);
		if (capital) {
			
			Vector3 vecToTarget = target.transform.position - transform.position;
			vecToTarget = Vector3.ClampMagnitude(vecToTarget, vecToTarget.magnitude/2);
			
			//Debug.DrawLine(transform.position, Vector3.Cross(vecToTarget, Vector3.up), Color.yellow);
			dv = Seek(Vector3.Cross(vecToTarget, Vector3.up));
		} 
		else {
			Vector3 changeInPos = target.velocity * frames;
			Vector3 futurPos = target.transform.position + changeInPos;
			dv = Seek (futurPos);
		}


		return dv;
	}

	protected Vector3 Evade(Vehicle target)
	{
		//Debug.DrawLine(transform.position, target.transform.position, Color.red);
		if(capital){
			
			Vector3 vecToTarget = target.transform.position - transform.position;
			vecToTarget = Vector3.ClampMagnitude(vecToTarget, vecToTarget.magnitude/2);
			
			//Debug.DrawLine(transform.position, Vector3.Cross(Vector3.up, vecToTarget), Color.yellow);
			dv = Flee(Vector3.Cross(Vector3.up, vecToTarget));
		}
		else{
		
			Vector3 changeInPos = target.velocity * frames;
			Vector3 futurPos = target.transform.position + changeInPos;
			dv = Flee (futurPos);
		}
		return dv;
	}

	public Vector3 Flee (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = transform.position - targetPos;		
		dv = dv.normalized * maxSpeed;
		dv -= characterController.velocity;

		return dv;
	}
	
	protected Vector3 AvoidObstacle (GameObject obst, float safeDistance)
	{ 

		dv = Vector3.zero;
		float obRadius = obst.GetComponent<ObstacleScript> ().Radius;
		safeDistance = radius + obRadius + 5;
		
		//Create a vector from vehicle to center of obstacle
		Vector3 vecToCenter = obst.transform.position - transform.position; 
		
		float dist = Mathf.Max(vecToCenter.magnitude - obRadius - radius, 0.1f);

		
		// if obstacle is too far to worry about, return dv
		
		if(dist > safeDistance)
		{
			return dv;
		}
		
		//if obstacle is behind, return dv
		
		if (Vector3.Dot(vecToCenter, transform.forward) < 0) 
		{
			return Wander ();
		}
		
		
		//if we can pass safely, return dv
		
		float dotWithRight = Vector3.Dot(vecToCenter, transform.right);

		if (Mathf.Abs (dotWithRight) > obRadius + radius) 
		{
			return dv;
		}

		//if we are going to collide, decide which way to turn
		if (dotWithRight > 0) 
		{
			dv = transform.right * -maxSpeed * safeDistance/dist;
		} 
		else
		{
			dv = transform.right * maxSpeed * safeDistance/dist;
		}
		
		dv -= velocity;
		
		return dv;
	}	
	
	protected Vector3 Wander( )
	{

		Vector3 target = transform.position + transform.forward * wanderDist;
		Quaternion rot = Quaternion.Euler(0, wanderAng, 0);
		Vector3 offset = rot * transform.forward;
		target += offset * wanderRad;
		wanderAng += Random.Range (-wanderRand, wanderRand);
		return Seek (target);
	}
	
	public Vector3 Separation(List<GameObject> neighbors, float separationDistance)
	{

		float distance = 100000.0f;
		GameObject nearest = null;

		if (neighbors.Count == 0) {
		
			return Vector3.zero;
		}

		for (int i = 0; i<neighbors.Count; i++)
		{
			if (!neighbors[i].Equals(this.gameObject))
			{
				if(neighbors[i] == null){
				
					neighbors.RemoveAt(i);
					continue;
				}

				if (nearest == null || distance > Vector3.Distance(neighbors[i].transform.position, this.gameObject.transform.position))
				{
					distance = Vector3.Distance(neighbors[i].transform.position, this.gameObject.transform.position);
					nearest = neighbors[i];
				}
			}
		}
		
		if (distance < separationDistance)
		{
			return Flee(nearest.transform.position);
		}
		else
			return Vector3.zero;
	}
	
	public Vector3 Alignment(Vector3 direction)
	{
		dv = direction.normalized * maxSpeed;
		dv -= velocity;
		return dv;
	}
	
	public Vector3 Cohesion(Vector3 targetPos)
	{
		return Seek(targetPos);
	}
	
	public Vector3 StayInBounds(float radius, Vector3 center)
	{
		//if out of bounds, seek center
		if(Vector3.Distance(transform.position, center) > radius){
			return Seek(center);
		}
		else
			return Vector3.zero;
	}
	
}
