using UnityEngine;
using System.Collections;

public class TimerSpotlight : MonoBehaviour {

	public GameObject lightPrefab;
	//public GameObject player;

	public bool spotlightCreated;

	private MovingLight ml;
	private bool turnBack;
	private GameObject[] points = new GameObject[1];



	// Use this for initialization
	void Start () {

		spotlightCreated = false;
		turnBack = false;
	}


	public void CreateSpotlight(float y){

		if (!spotlightCreated) {

			spotlightCreated = true;

			Vector3 startPos = new Vector3 (-0.2f, 0.0f, 0.0f);
			startPos = Camera.main.ViewportToWorldPoint (startPos);
			startPos.y = y;//player.transform.position.y;
			startPos.z = -4;

			Vector3 endPos = new Vector3 (1.2f, 0.0f, 0.0f);
			endPos = Camera.main.ViewportToWorldPoint (endPos);

			endPos.y = y;//player.transform.position.y;
			endPos.z = -4;

			GameObject wps = new GameObject ();

			GameObject startGo = new GameObject ();
			startGo.transform.position = startPos;
			startGo.transform.parent = wps.transform;

			GameObject endGo = new GameObject ();
			endGo.transform.position = endPos;
			endGo.transform.parent = wps.transform;

			points [0] = wps;

			GameObject light = (GameObject)Instantiate (lightPrefab, startPos, Quaternion.identity);
			light.GetComponent<MovingLight> ().SetWaypoints (wps);
			ml = light.GetComponent<MovingLight> ();
			//wps.transform.parent = light.transform;
		}

	}

	public void FixedUpdate(){

		if(turnBack && ml != null && ml.LightIsAtWaypoint()){

			Destroy (ml.gameObject);
			spotlightCreated = false;
			turnBack = false;
			Destroy(points[0]);

		}



	}

	public void TurnBack(){
		if (spotlightCreated && !turnBack) {
			ml.TurnBack ();
			turnBack = true;
		}
	}

}
