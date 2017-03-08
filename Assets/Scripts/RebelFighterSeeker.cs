using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RebelFighterSeeker : Vehicle {

	public Vehicle target;
	private GameObject[] obstacles;
	private GameObject[] rebelCaps;
	private GameObject[] empireCaps;
	
	private bool attacking = true;
	
	public float seekWt = 75.0f;
	public float evadeWt = 2;
	public float avoidWt = 10.0f;
	public float avoidDist = 20.0f;
	public float avoidFighterDist = 5.0f;
	public float wanderWt = 10.0f;
	
	public float inBoundsWt = 1f;
	public float alignmentWt = 1f;
	public float separationWt = 1f;
	public float separationDist = 1f;
	public float cohesionWt = 1f;
	
	private GameManager gm;
	//---------------------------------------------------------
	
	override public void Start () {
		base.Start();
		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		rebelCaps = GameObject.FindGameObjectsWithTag ("EmpireCapitals");
		empireCaps = GameObject.FindGameObjectsWithTag ("RebelCapitals");
		gm = GameObject.Find("MainGO").GetComponent<GameManager>();
	}
	
	protected override void CalcSteeringForce(){
		
		Vector3 force = Vector3.zero;
		
		//float dist = Vector3.Distance (transform.position, target.transform.position);
		//force += seekWt * Seek(target.transform.position);
		
		if (target != null) 
		{
			force += seekWt * Pursue (target);
			
			force += alignmentWt * Alignment(gm.RebelFightersDirection);
			force += separationWt * Separation(gm.RebelFighters, separationDist);
			force += cohesionWt * Cohesion(gm.RebelFightersCentroid);
			force += inBoundsWt * StayInBounds(30, new Vector3(0, 1, 0));
			
			//avoid obstacles
			for (int i=0; i<obstacles.Length; i++) {	
				force += avoidWt * AvoidObstacle (obstacles [i], avoidDist);
			}
			
			//force += avoidWt * AvoidObstacle (target.gameObject, avoidFighterDist);
			
			for (int i=0; i<rebelCaps.Length; i++) {
				
				force += avoidWt * AvoidObstacle (rebelCaps[i], avoidDist);
			}
			
			for (int i=0; i<empireCaps.Length; i++) {
				
				force += avoidWt * AvoidObstacle (empireCaps[i], avoidDist);
			}
		}
		else {
		
			force += wanderWt * Wander();

			force += alignmentWt * Alignment(gm.RebelFightersDirection);
			force += separationWt * Separation(gm.RebelFighters, separationDist);
			force += cohesionWt * Cohesion(gm.RebelFightersCentroid);
			force += inBoundsWt * StayInBounds(30, new Vector3(0, 1, 0));
		
			//avoid obstacles
			for (int i=0; i<obstacles.Length; i++) {	
				force += avoidWt * AvoidObstacle (obstacles [i], avoidDist);
			}
			
			for (int i=0; i<rebelCaps.Length; i++) {
		
				force += avoidWt * AvoidObstacle (rebelCaps[i], avoidDist);
			}
		
			for (int i=0; i<empireCaps.Length; i++) {
			
				force += avoidWt * AvoidObstacle (empireCaps[i], avoidDist);
			}
		}
		
		//limit force to maxForce and apply
		force = Vector3.ClampMagnitude (force, maxForce);
		ApplyForce(force);
		
		Debug.DrawLine(transform.position, transform.position - force,Color.blue);
		
	}

	void OnParticleCollision(GameObject other){
	
		Health -= 10;
		
		if (Health < 0) {
		
			Health = 0;
			Destroy(gameObject);
		}
	}
}
