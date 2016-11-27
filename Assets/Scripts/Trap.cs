using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            // Kill the player
            var player = collider.transform.GetComponent<CharacterMovement>();
            player.KillByTrap();

            // Play sound

            // Play animation
            var animator = GetComponent<Animator>();
            animator.SetTrigger("Close");
        }
    }
}
