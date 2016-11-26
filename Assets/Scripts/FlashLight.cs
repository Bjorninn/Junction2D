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


	// Use this for initialization
	void Start ()
	{
	    Instance = this;
	    CurrentCharge = MaxCharge;
	    _drainRate = 10.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.F))
	    {
	        ToggleFlashlight();
	    }
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

            Debug.Log("Current Charge is: " + CurrentCharge);

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

        Beam.gameObject.SetActive(_isOn);

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
