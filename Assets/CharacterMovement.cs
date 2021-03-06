﻿using UnityEngine;
using UnityEngine.UI;
public class CharacterMovement : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		timescaleDefault = Time.timeScale;
	}

	[HideInInspector]
	public bool facingRight = true;

	public int actualDirectionVector
	{
		get { return facingRight ? -1 : 1; }
		private set { }
	}

	[HideInInspector]
	public bool doJump = false;
	public float JumpVerticalForce = 1000f;
	public Transform groundCheck;
	public float speed = 20;
	public float slideForce = 100;
	public float slideDuration = 2;
	public GameObject timerSpotlight;

	private bool isGrounded = false;
	private Animator anim;
	private Rigidbody2D rb2d;
	private Transform tran;
	private PolygonCollider2D polyCollider;
	private AudioSource audio;

	private bool doSlide = false;
	public bool isSliding = false;
	private float slideStart;

	public float deathTimer = 10f;
	public bool isDead { get; private set; }

	private bool idleMove;
	private bool tryingToJump;
	private float idleTimeLimit;
	private float idleTimeCounter;

	private bool mainMenuEnabled = false;
    private bool gameOverScreenEnabled = false;
    private float timescaleDefault;
	public bool isJumping { get; private set; }
    public bool isRunning { get; private set; }
	public float chasersPosition;
	public float playersAdvance = 100;
	public float chasersSpeed = 1;
	private bool doLongJump;

	public PolygonCollider2D slidingCollider;
	private PolygonCollider2D tempCollider;
	private Vector2[] oldPath;

    public AudioClip reloadingSound;
    public AudioClip shootingSound;

    private int playerScore = 0;
    private bool isFalling;

    // Use this for initialization
    void Awake()
	{
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		tran = GetComponent<Transform>();
		polyCollider = GetComponent<PolygonCollider2D>();
		audio = GetComponent<AudioSource>();

		idleTimeLimit = 5.0f;
		idleTimeCounter = 0.0f;
		chasersPosition = tran.position.x - playersAdvance;
    }

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !gameOverScreenEnabled)
		{
			PauseGameAndOpenMenu();
		}

		isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		var v = Input.GetAxis("Vertical");
		var h = Input.GetAxis("Horizontal");

	    if (rb2d.velocity.y < 0)
	    {
	        isFalling = true;
	    }

		if (isGrounded && isFalling)
		{
		    isFalling = false;
			StopJumping();
		}
		else
		{
		    anim.SetBool("IsJumping", true);
		}

		if (isGrounded && !isSliding && v != 0)
		{

			//We can only jump or slide, not both
			if (v > 0)
			{

					doJump = true;
			
			}
			else if (v < 0)
			{
				doSlide = true;
			}
		}

		MoveChaser();

	    //AmplifyFall();

	}

    private void AmplifyFall()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 1.2f);
        }
    }

    private void StopJumping()
	{
	    if (isJumping)
	    {
            isJumping = false;
            anim.SetBool("IsJumping", false);
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }
	}

	private void MoveChaser()
	{
		chasersPosition += Time.deltaTime*chasersSpeed;

	}


	public void Kill()
	{
		if (!isDead)
		{
            audio.Stop();
            isDead = true;
            this.StopRunning();
            this.StopJumping();
            anim.SetTrigger("Idle");
            gameOverScreenEnabled = true;
            //anim.Stop();

            audio.volume = 0.6f;
			audio.PlayOneShot(reloadingSound, 0.1f);            
			Invoke("_Kill", 1.0f);
		}
	}

    public void KillByTrap()
    {
        if (!isDead)
        {
            audio.Stop();
            isDead = true;
            this.StopRunning();
            this.StopJumping();
            anim.SetTrigger("Idle");
            gameOverScreenEnabled = true;
            //anim.Stop();

            audio.volume = 0.6f;

            anim.SetTrigger("Death");
            audio.volume = 0.9f;
            //timerSpotlight.GetComponent<TimerSpotlight>().TurnBack();

            if (mainMenuEnabled)
            {
                Time.timeScale = 0;
            }

            GameObject gameOverMenu = GameObject.FindGameObjectWithTag("GameOver");
            Transform[] components = gameOverMenu.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in components)
            {
                t.gameObject.SetActive(true);
            }

            GameObject scoreLabel = GameObject.FindGameObjectWithTag("ScoreLabel");
            var textComponent = scoreLabel.GetComponent<Text>();
            //textComponent.text = "Flashlight charge: " + playerScore.ToString();
        }
    }

    private void _Kill()
    {
        anim.SetTrigger("Death");
        audio.volume = 0.9f;
        audio.PlayOneShot(shootingSound, 0.5f);
        //timerSpotlight.GetComponent<TimerSpotlight>().TurnBack();

        if (mainMenuEnabled)
        {
            Time.timeScale = 0;
        }

        GameObject gameOverMenu = GameObject.FindGameObjectWithTag("GameOver");
        Transform[] components = gameOverMenu.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in components)
        {
            t.gameObject.SetActive(true);
        }

        GameObject scoreLabel = GameObject.FindGameObjectWithTag("ScoreLabel");
        var textComponent = scoreLabel.GetComponent<Text>();
        //textComponent.text = "Flashlight charge: " + playerScore.ToString();
    }

	void FixedUpdate()
	{
		var h = Input.GetAxis("Horizontal");
		if (isDead) return;
		if (doJump)
		{
			StopRunning();
			Jump();
		}
		
		else if (doSlide)
		{
			StopRunning();
			Slide();
		}
		if (!isSliding)
		{

			//TODO: replace this
			/*
			 if(Mathf.Abs(h) < 0.05f){

				idleMove = true;

			} else {

				idleMove = false;
			}
			 */

			if (h != 0f)
			{
				MoveInPressedDirectionAndFlipIfNecessary(h);
			}
			else if (h == 0 && isRunning)
			{
				StopRunning();
			}
		}




		if (chasersPosition>= tran.position.x)
		{
			//timerSpotlight.GetComponent<TimerSpotlight>().CreateSpotlight(tran.position.y);
		}


		if (!idleMove || tryingToJump)
		{
			// if player moves,reset timer
			// reset
			idleTimeCounter = 0;
			//timerSpotlight.GetComponent<TimerSpotlight>().TurnBack();
		}
	}

	private void StopSliding()
	{
		isSliding = false;
		rb2d.velocity = new Vector2(0, rb2d.velocity.y); //we don't want to prevent it from stopping falling
		SetStandingTransform();
	}

	private void StopRunning()
	{
		isRunning = false;
		anim.SetBool("Running", false);
	}

	private void MoveInPressedDirectionAndFlipIfNecessary(float h)
	{
		tran.Translate(Vector3.right*h*Time.deltaTime*speed);

		if (h < 0 && !facingRight)
			Flip();
		else if (h > 0 && facingRight)
			Flip();
		
	    if (!isJumping && !doJump)
	    {
            isRunning = true;
            anim.SetBool("Running", true);
        }
	}

	private void Jump()
	{
		if(!isJumping)
        { anim.SetTrigger("Jumping");}

        //rb2d.AddForce(new Vector2(highJumpHorizontalForce*actualDirectionVector, 0f), ForceMode2D.Impulse);
        doJump = false;
        isJumping = true;
        
    }

    private void AddForceToJump()
    {
        
        rb2d.AddForce(new Vector2(0f, JumpVerticalForce), ForceMode2D.Impulse);
        //rb2d.velocity = new Vector2(highJumpHorizontalForce*actualDirectionVector, highJumpVerticalForce);

      
    }

   



	private void Slide()
	{
		slideStart = Time.time;
		anim.SetBool("Running", false);
		anim.SetTrigger("Sliding");

        isSliding = true;
        doSlide = false;
		
	}

    private void AddSlideForce()
    {
        rb2d.AddForce(new Vector2(slideForce * actualDirectionVector, 0f), ForceMode2D.Impulse); ;
        
        SetSlidingTransform();
    }


	public void Die()
	{

		Destroy(this.gameObject);
		timerSpotlight.GetComponent<TimerSpotlight>().TurnBack();
	}



	private void SetSlidingTransform()
	{
		//polyCollider.SetPath(0, new Vector2[] {new Vector2(-3.6f, 0), new Vector2(-3.6f, -4.92f), new Vector2(3.6f, -4.92f), new Vector2(3.6f, 0)});
		oldPath = polyCollider.GetPath(0);
		polyCollider.SetPath(0, slidingCollider.GetPath(0));

	}

	private void SetStandingTransform()
	{
		polyCollider.SetPath(0, oldPath);
		
	}


	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void PauseGameAndOpenMenu()
	{
		GameObject mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
		Transform[] components = mainMenu.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in components)
		{
			t.gameObject.SetActive(!mainMenuEnabled);
		}
		mainMenu.SetActive(true);
		mainMenuEnabled = !mainMenuEnabled;

		if (mainMenuEnabled)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = timescaleDefault;
		}
	}
}
