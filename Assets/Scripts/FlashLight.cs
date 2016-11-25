using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour
{
    public static FlashLight Instance { private set; get; }

    public float MaxCharge;
    private float _currentCharge;
    private float _drainRate;

    private bool _isOn;


	// Use this for initialization
	void Start ()
	{
	    Instance = this;
	    _currentCharge = MaxCharge;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

    public void AddCharge(float charge)
    {
        _currentCharge += charge;

        if (_currentCharge > MaxCharge)
        {
            _currentCharge = MaxCharge;
        }
    }

    private IEnumerator Drain()
    {
        while (_isOn)
        {
            _currentCharge -= _drainRate;
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void ToggleFlashlight(bool isOn)
    {
        _isOn = isOn;

        if (isOn)
        {
            StartCoroutine(Drain());
        }
    }
}
