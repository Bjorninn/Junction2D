
			// if too long idle
			if (idleTimeCounter > idleTimeLimit) {
				timerSpotlight.GetComponent<TimerSpotlight> ().CreateSpotlight (tran.position.y);	
			}
		} 


		if (!idleMove || tryingToJump){
		// if player moves,reset timer
			// reset
			idleTimeCounter = 0;
			timerSpotlight.GetComponent<TimerSpotlight> ().TurnBack ();
		}
	}


	public void Die(){
	
		Destroy (this.gameObject);
		timerSpotlight.GetComponent<TimerSpotlight> ().TurnBack ();
	}


	private void SetSlidingTransform()
	{
		box.size = new Vector2(2, 0.5f);
	}

	private void SetStandingTransform()
	{
		box.size = new Vector3(1, 1);
	}


	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
