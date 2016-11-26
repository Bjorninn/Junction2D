using UnityEngine;
using System.Collections;

public class MovingLight : MonoBehaviour {

	public Color lightColor; // color of this light
	//public float spotAngle = 30.0f;	
	public float speed = 10.0f;
	public GameObject waypoints;

	protected Light m_light; // light
	protected int currentLight;
	protected Transform pos;
	protected Vector2 pos2d = new Vector2();
	protected float radius;

	// Use this for initialization
	void Start () {

		m_light = GetComponent<Light> ();
		m_light.color = lightColor;
		//light.spotAngle = spotAngle;

		currentLight = 0;

		pos = GetComponent<Transform> ();
		radius = pos.FindChild ("Range").gameObject.GetComponent<CircleCollider2D> ().radius;

	}

	public void FixedUpdate(){

		MoveLightWaypoints ();

		pos2d.x = pos.position.x;
		pos2d.y = pos.position.y;

		Collider2D[] colliders = Physics2D.OverlapCircleAll (pos2d, radius);

		for (int i = 0; i < colliders.Length; i++) {

			Collider2D collider = colliders [i];

			//Debug.Log (colliders [i].gameObject.name);

			if (collider.gameObject.tag == "Player") {

			//	Destroy (collider.gameObject);
				collider.gameObject.GetComponent<CharacterMovement>().Die(); // TODO to Kill()
			}
		}

	}

	public void MoveLightWaypoints(){

		Transform target = waypoints.transform.GetChild (currentLight);
		pos.position = Vector3.MoveTowards (pos.position, target.position, speed * Time.deltaTime); 

		// if we are at the target
		if ((target.position - pos.position).magnitude == 0.0f) {
			currentLight = (currentLight + 1) % waypoints.transform.childCount;
		}
	}

	public bool LightIsAtWaypoint(){
		Transform target = waypoints.transform.GetChild (currentLight);
		pos.position = Vector3.MoveTowards (pos.position, target.position, speed * Time.deltaTime); 

		// if we are at the target
		return (target.position - pos.position).magnitude == 0.0f;

	}

	public void SetWaypoints(GameObject wps){

		this.waypoints = wps;
	}

	public void TurnBack(){
	
	
		this.waypoints.transform.GetChild (1).transform.position = this.waypoints.transform.GetChild (0).transform.position;
	}
}
