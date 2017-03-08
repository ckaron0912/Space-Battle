using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RebelCapSeeker : Vehicle {
	public Vehicle target;
	private  GameObject[] obstacles; 
	
	private bool attacking = false;

	public float seekWt = 75.0f;
	public float evadeWt = 3.0f;
	public float avoidWt = 10.0f;
	public float avoidDist = 20.0f;
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
        gm = GameObject.Find("MainGO").GetComponent<GameManager>();
		Capital = true;
	}

	protected override void CalcSteeringForce(){

		Vector3 force = Vector3.zero;	
		float dist = Vector3.Distance (transform.position, target.transform.position);
		//force += seekWt * Seek(target.transform.position);
		if (dist < 150) 
		{
			attacking = false;
		} 
		else if(dist > 150)
		{
			attacking = true;
		}
		
		if (attacking) 
		{
			force += seekWt * Pursue (target);
		}
		else force += evadeWt * Evade (target);

		//force += seekWt * Seek(target.transform.position);
        force += alignmentWt * Alignment(gm.RebelCapsDirection);
		force += separationWt * Separation(gm.RebelCaps, separationDist);
        force += cohesionWt * Cohesion(gm.RebelCapsCentroid);
        force += inBoundsWt * StayInBounds(200, new Vector3(0, 1, 0));

		//avoid obstacles
		for (int i=0; i<obstacles.Length; i++) {	
			force += avoidWt * AvoidObstacle (obstacles [i], avoidDist);
		}
		
		//limit force to maxForce and apply
		force = Vector3.ClampMagnitude (force, maxForce);
		ApplyForce(force);

		Debug.DrawLine(transform.position, transform.position - force,Color.blue);

	}
}
