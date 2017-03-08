using UnityEngine;
using System.Collections;

public class EmpireCannonController : MonoBehaviour {

	float min = -17.0f;
	float max = 190.0f;
	public Vehicle target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(target);
		
		target = transform.parent.gameObject.GetComponent<EmpireCapSeeker>().target;

		if(target != null){
		
			Vector3 direction = target.transform.position - transform.position;
			Vector3.Normalize(direction);
			Vector3 rot = transform.localRotation.eulerAngles;
			rot.y = direction.y;
			transform.rotation = Quaternion.Euler(rot);

			//transform.Rotate();
			//transform.rotation = Quaternion.
			//transform.localRotation = Quaternion.Euler(0, Mathf.Clamp((int)transform.rotation.y, min, max), 0);
			//transform.rotation = Quaternion.AngleAxis(
		}
	}
}
