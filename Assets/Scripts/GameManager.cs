
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject ObstaclePrefab;
	private  GameObject[] obstacles; 

	private int cameraIndex = 0;

    public int numberFlockers = 1;
	private List<GameObject> rebelCaps;
	private List<GameObject> rebelFighters;
	private List<GameObject> empireTies;
	private List<GameObject> empireCaps;
	private List<GameObject> cameras;

	public List<GameObject> RebelCaps { get { return rebelCaps; } }
	public List<GameObject> RebelFighters { get { return rebelFighters; } }
	public List<GameObject> EmpireTies { get { return empireTies; } }
	public List<GameObject> EmpireCaps { get { return empireCaps; } }

    private Vector3 rebelCapsDirection;
	public Vector3 RebelCapsDirection { get { return rebelCapsDirection; } }
	
	private Vector3 rebelFightersDirection;
	public Vector3 RebelFightersDirection { get { return rebelFightersDirection; } }

	private Vector3 empireCapsDirection;
	public Vector3 EmpireCapsDirection { get { return empireCapsDirection; } }

	private Vector3 empireTiesDirection;
	public Vector3 EmpireTiesDirection { get { return empireTiesDirection; } }

    private Vector3 rebelCapsCentroid;
	public Vector3 RebelCapsCentroid { get { return rebelCapsCentroid; } }
	
	private Vector3 rebelFightersCentroid;
	public Vector3 RebelFightersCentroid { get { return rebelFightersCentroid; } }

	private Vector3 empireCapsCentroid;
	public Vector3 EmpireCapsCentroid { get { return empireCapsCentroid; } }

	private Vector3 empireTiesCentroid;
	public Vector3 EmpireTiesCentroid { get { return empireTiesCentroid; } }
	
	void Start () {

		rebelCaps = new List<GameObject>(GameObject.FindGameObjectsWithTag("RebelCapitals"));
		rebelFighters = new List<GameObject>(GameObject.FindGameObjectsWithTag("RebelFighters"));
		empireTies = new List<GameObject>(GameObject.FindGameObjectsWithTag("EmpireTie"));
		empireCaps = new List<GameObject>(GameObject.FindGameObjectsWithTag("EmpireCapitals"));

		
		foreach (GameObject i in empireTies) 
		{
			i.GetComponent<TieFighterSeeker>().target = rebelFighters[0].GetComponent<RebelFighterSeeker>();
		}
		
		foreach (GameObject i in empireCaps) 
		{
			i.GetComponent<EmpireCapSeeker>().target = rebelCaps[0].GetComponent<RebelCapSeeker>();
		}

		foreach (GameObject i in rebelCaps) 
		{
			i.GetComponent<RebelCapSeeker>().target = empireCaps[0].GetComponent<EmpireCapSeeker>();
		}
		
		foreach (GameObject i in rebelFighters) 
		{
			i.GetComponent<RebelFighterSeeker>().target = empireTies[0].GetComponent<TieFighterSeeker>();
		}

		foreach (GameObject i in rebelCaps) 
		{
			float prevDist = Vector3.Distance(i.transform.position, i.GetComponent<RebelCapSeeker>().target.transform.position);

			foreach (GameObject j in empireCaps) 
			{
				float dist = Vector3.Distance(i.transform.position, j.transform.position);
				
				if(dist < prevDist)
				{
					
					i.GetComponent<RebelCapSeeker>().target = j.GetComponent<EmpireCapSeeker>();
				}
			}
		}

		foreach (GameObject i in rebelFighters) 
		{
			float prevDist = Vector3.Distance(i.transform.position, i.GetComponent<RebelFighterSeeker>().target.transform.position);
			
			foreach (GameObject j in empireTies) 
			{
				float dist = Vector3.Distance(i.transform.position, j.transform.position);
				
				if(dist < prevDist)
				{
					
					i.GetComponent<RebelFighterSeeker>().target = j.GetComponent<TieFighterSeeker>();
				}
			}
		}

		foreach (GameObject i in empireCaps) 
		{
			float prevDist = Vector3.Distance(i.transform.position, i.GetComponent<EmpireCapSeeker>().target.transform.position);
			
			foreach (GameObject j in rebelCaps) 
			{
				float dist = Vector3.Distance(i.transform.position, j.transform.position);
				
				if(dist < prevDist)
				{
					
					i.GetComponent<EmpireCapSeeker>().target = j.GetComponent<RebelCapSeeker>();
				}
			}
		}

		foreach (GameObject i in empireTies) 
		{
			float prevDist = Vector3.Distance(i.transform.position, i.GetComponent<TieFighterSeeker>().target.transform.position);

			foreach(GameObject j in rebelFighters)
			{
				float dist = Vector3.Distance(i.transform.position, j.transform.position);

				if(dist < prevDist)
				{

					i.GetComponent<TieFighterSeeker>().target = j.GetComponent<RebelFighterSeeker>();
				}
			}
		}

		//make some obstacles
		/*for (int i=0; i< 30; i++) 
		{
			pos =  new Vector3(Random.Range(-40, 40), Terrain.activeTerrain.SampleHeight(pos), Random.Range(-40, 40));
			Quaternion rot = Quaternion.Euler(0, Random.Range(0, 90), 0);
			GameObject.Instantiate(ObstaclePrefab, pos, rot);
		}*/
		obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");

        //camera follow
        //Camera.main.GetComponent<SmoothFollow>().target = gameObject.transform;
		cameras = new List<GameObject>(GameObject.FindGameObjectsWithTag("Camera"));

		cameras [1].GetComponent<SmoothFollow> ().target = GameObject.Find("RebelCaps").transform;	
		cameras [2].GetComponent<SmoothFollow> ().target = GameObject.Find("EmpireCaps").transform;	
		cameras [0].GetComponent<SmoothFollow> ().target = GameObject.Find("RebelFighters").transform;
		cameras [4].GetComponent<SmoothFollow> ().target = GameObject.Find("TieFighters").transform;	
	}

	void SwitchCamera()
	{
		if(cameraIndex > cameras.Count - 1)
		{
			cameraIndex = 0;
		}
		else if(cameraIndex < 0)
		{
			cameraIndex = cameras.Count - 1;
		}

		for(int i = 0; i < cameras.Count; i++)
		{
			if(i == cameraIndex)
			{
				cameras[i].SetActive(true);
			}
			else cameras[i].SetActive(false);
		}
	}

	void Update () {

		if(Input.GetKeyDown(KeyCode.Comma))
		{
			cameraIndex--;
			SwitchCamera();
		}
		else if(Input.GetKeyDown(KeyCode.Period))
		{
			cameraIndex++;
			SwitchCamera();
		}

        //calculate the centroid and flock direction
		calcRebelCapsCentroid();
		calcRebelFightersCentroid();
		calcEmpireCapsCentroid();
		calcEmpireTiesCentroid();

		calcRebelCapsDirection();
		calcRebelFightersDirection();
		calcEmpireCapsDirection();
		calcEmpireTiesDirection();
		AcuireTargets();
	}
	
	private void calcRebelCapsCentroid()
	{
		rebelCapsCentroid = Vector3.zero;
		foreach(GameObject f in rebelCaps){
			rebelCapsCentroid += f.transform.position;
		}
		rebelCapsCentroid /= RebelCaps.Count;
		GameObject.Find("RebelCaps").transform.position = rebelCapsCentroid;
	}

	private void calcRebelFightersCentroid()
	{
		rebelFightersCentroid = Vector3.zero;

		for(int i = 0; i < rebelFighters.Count; i++){
			//Debug.Log ("rebel Fighters left: " + rebelFighters.Count);

			if (rebelFighters[i] == null) {

				rebelFighters.RemoveAt(i);
				continue;
			}

			rebelFightersCentroid += rebelFighters[i].transform.position;
		}

		if(rebelFighters.Count == 0){

			return;
		}
		rebelFightersCentroid /= RebelFighters.Count;
		GameObject.Find("RebelFighters").transform.position = rebelFightersCentroid;
	}

	private void calcEmpireCapsCentroid()
	{
		empireCapsCentroid = Vector3.zero;
		foreach(GameObject f in empireCaps){
			empireCapsCentroid += f.transform.position;
		}
		empireCapsCentroid /= EmpireCaps.Count;
		GameObject.Find("EmpireCaps").transform.position = empireCapsCentroid;
	}

	private void calcEmpireTiesCentroid()
	{
		if(empireTies.Count == 0){
			return;
		}
		
		empireTiesCentroid = Vector3.zero;
		foreach(GameObject f in empireTies){
			
			if(f == null){
				
				continue;
			}
			empireTiesCentroid += f.transform.position;
		}
		empireTiesCentroid /= EmpireTies.Count;
		GameObject.Find("TieFighters").transform.position = empireTiesCentroid;
	}

    private void calcRebelCapsDirection()
    {
        rebelCapsDirection = Vector3.zero;
		foreach (GameObject f in rebelCaps)
		{
			rebelCapsDirection += f.transform.forward;
        }
		rebelCapsDirection /= RebelCaps.Count;
		GameObject.Find("RebelCaps").transform.forward = rebelCapsDirection;
    }

	private void calcRebelFightersDirection()
	{
		rebelFightersDirection = Vector3.zero;
		foreach (GameObject f in rebelFighters)
		{
			rebelFightersDirection += f.transform.forward;
		}
		rebelFightersDirection /= RebelFighters.Count;
		GameObject.Find("RebelFighters").transform.forward = rebelFightersDirection;
	}

	private void calcEmpireCapsDirection()
	{
		empireCapsDirection = Vector3.zero;
		foreach (GameObject f in empireCaps)
		{
			empireCapsDirection += f.transform.forward;
		}
		empireCapsDirection /= EmpireCaps.Count;
		GameObject.Find("EmpireCaps").transform.forward = empireCapsDirection;
	}

	private void calcEmpireTiesDirection()
	{
		if(empireTies.Count == 0){
			return;
		}
		empireTiesDirection = Vector3.zero;
		foreach (GameObject f in empireTies)
		{
			if(f == null){
				
				continue;
			}
			empireTiesDirection += f.transform.forward;
		}
		empireTiesDirection /= EmpireTies.Count;
		GameObject.Find("TieFighters").transform.forward = empireTiesDirection;
	}

	private void AcuireTargets()
	{

		foreach (GameObject i in empireTies) 
		{
			if(i == null){
				continue;
			}
			if(rebelFighters.Count > 0){

				i.GetComponent<TieFighterSeeker>().target = rebelFighters[0].GetComponent<RebelFighterSeeker>();

				float prevDist = Vector3.Distance(i.transform.position, i.GetComponent<TieFighterSeeker>().target.transform.position);
					
				foreach(GameObject j in rebelFighters)
				{

					float dist = Vector3.Distance(i.transform.position, j.transform.position);

					if(dist < prevDist)
					{
							
						i.GetComponent<TieFighterSeeker>().target = j.GetComponent<RebelFighterSeeker>();
					}
				}
			}
			else i.GetComponent<TieFighterSeeker>().target = null;
		}
		
		foreach (GameObject i in empireCaps) 
		{
			i.GetComponent<EmpireCapSeeker>().target = rebelCaps[0].GetComponent<RebelCapSeeker>();
		}
		
		foreach (GameObject i in rebelCaps) 
		{
			i.GetComponent<RebelCapSeeker>().target = empireCaps[0].GetComponent<EmpireCapSeeker>();
		}
		
		foreach (GameObject i in rebelFighters) 
		{
			if(empireTies.Count == 0){
				return;
			}
			if(empireTies.Count > 0 && empireTies[0] != null){
				
				if(i == null){
					continue;
				}
				i.GetComponent<RebelFighterSeeker>().target = empireTies[0].GetComponent<TieFighterSeeker>();
				float prevDist = Vector3.Distance(i.transform.position, i.GetComponent<RebelFighterSeeker>().target.transform.position);
				
				foreach (GameObject j in empireTies) 
				{
					float dist = Vector3.Distance(i.transform.position, j.transform.position);
					
					if(dist < prevDist)
					{
						
						i.GetComponent<RebelFighterSeeker>().target = j.GetComponent<TieFighterSeeker>();
					}
				}
			}
			else i.GetComponent<RebelFighterSeeker>().target = null;
		}
		
		foreach (GameObject i in rebelCaps) 
		{
			float prevDist = Vector3.Distance(i.transform.position, i.GetComponent<RebelCapSeeker>().target.transform.position);
			
			foreach (GameObject j in empireCaps) 
			{
				float dist = Vector3.Distance(i.transform.position, j.transform.position);
				
				if(dist < prevDist)
				{
					
					i.GetComponent<RebelCapSeeker>().target = j.GetComponent<EmpireCapSeeker>();
				}
			}
		}
		
		foreach (GameObject i in empireCaps) 
		{
			float prevDist = Vector3.Distance(i.transform.position, i.GetComponent<EmpireCapSeeker>().target.transform.position);
			
			foreach (GameObject j in rebelCaps) 
			{
				float dist = Vector3.Distance(i.transform.position, j.transform.position);
				
				if(dist < prevDist)
				{
					
					i.GetComponent<EmpireCapSeeker>().target = j.GetComponent<RebelCapSeeker>();
				}
			}
		}

	}
}
