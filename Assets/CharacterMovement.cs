using UnityEngine;
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
	private bool isSliding = false;
	private float slideStart;

	public float deathTimer = 10f;
	public bool isDead { get; private set; }

	private bool idleMove;
	private bool tryingToJump;
	private float idleTimeLimit;
	private float idleTimeCounter;

	private bool mainMenuEnabled = false;
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
		if (Input.GetKeyDown(KeyCode.F))
		{
			PauseGameAndOpenMenu();
		}

		isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		var v = Input.GetAxis("Vertical");
		var h = Input.GetAxis("Horizontal");

		if (isGrounded && isJumping)
		{
			StopJumping();
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
		isJumping = false;
		anim.SetBool("Jumping", false);
		rb2d.velocity = new Vector2(0, rb2d.velocity.y);
	}

	private void MoveChaser()
	{
		chasersPosition += Time.deltaTime*chasersSpeed;

	}


	public void Kill()
	{
		if (!isDead)
		{
			isDead = true;
			audio.PlayOneShot(reloadingSound, 0.1f);            
			Invoke("_Kill", 1.0f);
		}
	}

	private void _Kill()
	{
		anim.SetTrigger("Death");
		audio.PlayOneShot(shootingSound, 0.5f);
		timerSpotlight.GetComponent<TimerSpotlight>().TurnBack();
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

		var slideTimeIsOver = isSliding && Time.time - slideStart >= slideDuration;
		if (slideTimeIsOver)
		{
			StopSliding();
		}



		if (chasersPosition>= tran.position.x)
		{
			timerSpotlight.GetComponent<TimerSpotlight>().CreateSpotlight(tran.position.y);
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
		isRunning = true;
		anim.SetBool("Running", true);
	}

	private void Jump()
	{
		anim.SetBool("Jumping", true);
		//rb2d.AddForce(new Vector2(highJumpHorizontalForce*actualDirectionVector, 0f), ForceMode2D.Impulse);
		rb2d.AddForce(new Vector2(0f, JumpVerticalForce), ForceMode2D.Impulse);
	    //rb2d.velocity = new Vector2(highJumpHorizontalForce*actualDirectionVector, highJumpVerticalForce);

        doJump = false;
		isJumping = true;
	}

   



	private void Slide()
	{
		slideStart = Time.time;
		anim.SetBool("Running", false);
		anim.SetTrigger("Sliding");
		rb2d.AddForce(new Vector2(slideForce*actualDirectionVector, 0f), ForceMode2D.Impulse);;
		isSliding = true;
		doSlide = false;
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
