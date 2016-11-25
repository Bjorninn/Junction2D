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
        // Get the flashlight

        // Increase the power of the flashlight by ChargeAmount
        FlashLight.Instance.AddCharge(ChargeAmount);

        // Do something else if we so want

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
