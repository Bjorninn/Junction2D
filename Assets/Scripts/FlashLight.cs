using System;
using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour
{
    public static FlashLight Instance { private set; get; }

    public float MaxCharge;
    public Light Beam;
    public float CurrentCharge { private set; get; }
    private float _drainRate;

    private bool _isOn;
    private float _maxWobble;
    private float _wobbleSpeed;
    private bool _isWobbling;

	// Use this for initialization
	void Start ()
	{
	    Instance = this;
	    CurrentCharge = MaxCharge;
	    _drainRate = 1.0f;
	    _isOn = Beam.enabled;

	    _maxWobble = 2.1f;
	    _wobbleSpeed = 0.1f;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.L))
	    {
	        ToggleFlashlight();
	    }

	    // Get player velocity
	    Vector2 velocity = gameObject.GetComponentInParent<Rigidbody2D>().velocity;
        
	    // Check if player is jumping or not
	    if (velocity.y > 0)
	    {

	    }
        Debug.Log("V> " + velocity);
	    // If player V > X then wobble the light
	    if (Math.Abs(velocity.x) > 0)
	    {
	        _isWobbling = true;

	        Vector3 pos = transform.position;

	        if (Math.Abs(pos.y) >= _maxWobble)
	        {
	            _wobbleSpeed *= -1;
	        }

	        pos.y += _wobbleSpeed;
	        transform.position = pos;
	    }
	    else
	    {
	        _isWobbling = false;
	    }

	    // if V is < X then check for wobble

	    // If player is jumping then do Z
	}

    public void AddCharge(float charge)
    {
        CurrentCharge += charge;

        if (CurrentCharge > MaxCharge)
        {
            CurrentCharge = MaxCharge;
        }
    }

    private IEnumerator Drain()
    {
        while (_isOn)
        {
            CurrentCharge -= _drainRate;

            if (CurrentCharge <= 0)
            {
                CurrentCharge = 0;
                ToggleFlashlight();
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    public void ToggleFlashlight()
    {
        _isOn = !_isOn;

        Beam.enabled = _isOn;

        if (CurrentCharge <= 0 && _isOn)
        {
            // If the player tries to turn the light on but it has no charge
            // then just turn it off again.
            ToggleFlashlight();
        }
        else if (_isOn)
        {
            StartCoroutine(Drain());
        }
    }
}
