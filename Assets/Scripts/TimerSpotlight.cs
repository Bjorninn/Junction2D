using UnityEngine;
using System.Collections;

public class TimerSpotlight : MonoBehaviour {

	public GameObject lightPrefab;
	public GameObject player;

	private MovingLight ml;

	// Use this for initialization
	void Start () {
	
		Vector3 startPos = new Vector3(-0.2f, 0.0f, 0.0f);
		startPos = Camera.main.ViewportToWorldPoint(startPos);
		startPos.y = player.transform.position.y;
	//	startPos.z = 0;


		Vector3 endPos = new Vector3(1.2f, 0.0f, 0.0f);
		endPos = Camera.main.ViewportToWorldPoint(endPos);

		endPos.y = player.transform.position.y;
	//	endPos.z = 0;


		GameObject wps = new GameObject ();

		GameObject startGo = new GameObject ();
		startGo.transform.position = startPos;
		startGo.transform.parent = wps.transform;

		GameObject endGo = new GameObject ();
		endGo.transform.position = endPos;
		endGo.transform.parent = wps.transform;

		GameObject light = (GameObject) Instantiate (lightPrefab, startPos, Quaternion.identity);
		light.GetComponent<MovingLight>().SetWaypoints(wps);
		ml = light.GetComponent<MovingLight> ();
	}

	public void FixedUpdate(){

		if (Input.GetKey (KeyCode.Space)) {
			ml.TurnBack ();
		}

	}

}
