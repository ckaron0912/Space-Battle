using UnityEngine;
using System.Collections;

public class TieFireController : MonoBehaviour {

	private GameObject parent;
	private Vehicle target;
	public float rangeToFire = 10.0f;

	// Use this for initialization
	void Start () {
	
		parent = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {

		
		target = parent.GetComponent<TieFighterSeeker> ().target;
		if (target == null) {
			
			GetComponent<ParticleSystem>().Stop();
			return;
		}

		Vector3 vecToCenter = target.transform.position - transform.position;

		RaycastHit hit;
		
		if(Physics.Raycast(parent.transform.position, parent.transform.forward,out hit, rangeToFire)){
			
			if(hit.collider.gameObject.tag == "RebelFighters"){
				
				GetComponent<ParticleSystem>().Play();
			}
		}
		else GetComponent<ParticleSystem>().Stop();
	}
}
