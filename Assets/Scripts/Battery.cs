using UnityEngine;
using System.Collections;

public class Battery : MonoBehaviour, IPickup
{

    public float ChargeAmount = 10.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPickUp()
    {
        // Get the flashlight and increase the power by ChargeAmount
        FlashLight.Instance.AddCharge(ChargeAmount);

        // Delete the battery
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            OnPickUp();
        }
    }
}
