﻿using UnityEngine;
using System.Collections;

public class PlatformScript : MonoBehaviour {
    private Rigidbody2D rb;
    private Vector3 position;
    private Quaternion rotation;
    private float fallingSpeed;
    private float rotatingSpeed;
    private bool breakable;

    enum FallingType {
        LEFT = 0,
        RIGHT = 1, 
        BOTH = 2,
        STATIC = 3
    }
    private FallingType platformFallingType;
    private int coef;
    private bool startFalling = false;
    private float secondsBeforeFallingDown;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();

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
        }
        else if (platformFallingType == FallingType.RIGHT)
        {
            coef = -1;
        }
        else
        {
            coef = 0;
        }

        secondsBeforeFallingDown = 6.0f;
        breakable = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (startFalling)
        {
            secondsBeforeFallingDown -= 0.25f;
            if (secondsBeforeFallingDown <= 0 && Time.timeScale > 0)
            {
                position.y -= fallingSpeed;
                this.transform.position = position;
                rotation.z += coef * rotatingSpeed;
                this.transform.rotation = rotation;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (breakable && !startFalling)
        {
            startFalling = true;
        }
    }

    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        
    }

    void OnCollisionExit2D(Collision2D collision)
    {

    }
}

