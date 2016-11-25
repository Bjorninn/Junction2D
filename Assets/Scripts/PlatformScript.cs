using UnityEngine;
using System.Collections;

public class PlatformScript : MonoBehaviour {
    public Rigidbody rb;
    private Vector3 position;
    private Quaternion rotation;
    private float fallingSpeed;
    private float rotatingSpeed;

    enum FallingType {
        LEFT = 0,
        RIGHT = 1, 
        BOTH = 2,
        STATIC = 3
    }

    private FallingType platformFallingType;
    private int coef;
    private bool startFalling = false;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.detectCollisions = true;

        position = transform.position;
        rotation = transform.rotation;
        // depends on count of flashlight charges ?
        rotatingSpeed = 0.005f;
        fallingSpeed = 0.1f;

        var values = FallingType.GetValues(typeof(FallingType));
        platformFallingType = (FallingType)values.GetValue(Random.Range(0, values.Length - 1));


        if (platformFallingType == FallingType.LEFT)
        {
            coef = 1;
        } else if (platformFallingType == FallingType.RIGHT)
        {
            coef = -1;
        } else
        {
            coef = 0;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (startFalling)
        {
            position.y -= fallingSpeed;
            this.transform.position = position;
            rotation.z += coef * rotatingSpeed;
            this.transform.rotation = rotation;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        startFalling = true;
        //rb.isKinematic = false;
        //rb.mass = 1.0f;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        
    }

    void OnCollisionExit(Collision collision)
    {

    }
}

