using UnityEngine;
public class CharacterMovement : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	[HideInInspector]
	public bool facingRight = true;

	public int actualDirectionVector
	{
		get {return facingRight ? 1 : -1; }
		private set { }
	}

	[HideInInspector]
	public bool jump = false;
	public float jumpVerticalForce = 1000f;
	public Transform groundCheck;
	public float speed = 20;
	public float slideForce = 100;
	public float slideDuration = 2;
	public GameObject timerSpotlight;

	private bool grounded = false;
	private Animator anim;
	private Rigidbody2D rb2d;
	private Transform tran;
	private BoxCollider2D box;
	private bool slide = false;
	private bool sliding = false;
	private float slideStart;
    public float deathTimer = 10f;
    private bool dead = false;
    public float jumpHorizontalForce = 1000f;

	private bool idleMove;
	private bool tryingToJump;
	private float idleTimeLimit;
	private float idleTimeCounter;

    // Use this for initialization
	void Awake()
	{
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		tran = GetComponent<Transform>();
		box = GetComponent<BoxCollider2D>();

		idleTimeLimit = 5.0f;
		idleTimeCounter = 0.0f;
	}

	// Update is called once per frame
	void Update()
	{
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
	    Stroke();
		var v = Input.GetAxis("Vertical");

	    if (grounded)
	    {
            anim.SetBool("Jumping", false);
        }

		if (Mathf.Abs (v) < 0.05f) {
			tryingToJump = false;
		} else {
			tryingToJump = true;
		}

		if (v > 0 && grounded)
		{
			jump = true;
		}
		else if (v < 0 && grounded && !sliding)
		{
			slide = true;
		}
	}

    public void Kill()
    {
        anim.SetTrigger("Death");
        dead = true;
		timerSpotlight.GetComponent<TimerSpotlight> ().TurnBack ();
    }

    void Stroke()
    {
      /**  if (Time.time > deathTimer)
        {
            Kill();
        }*/   
    }

    void FixedUpdate()
    {
        if (!dead)
        {
            if (!sliding)
            {
                if (grounded)
                {
                    float h = Input.GetAxis("Horizontal");
                    if (h != 0f)
                    {
                        anim.SetFloat("Speed", Mathf.Abs(h));

                        tran.Translate(Vector3.right * h * Time.deltaTime * speed);

                        if (h > 0 && !facingRight)
                            Flip();
                        else if (h < 0 && facingRight)
                            Flip();
                        if (grounded)
                        {
                            anim.SetBool("Running", true);
                        }
                        else
                        {
                            anim.SetBool("Running", false);
                        }
                    }
                    else
                    {
                        anim.SetBool("Running", false);
                    }
                }
            }
            else if (Time.time - slideStart >= slideDuration)

            {
                sliding = false;

                rb2d.velocity = Vector2.zero;
                //SetStandingTransform();

            }


            if (jump)
            {
                anim.SetBool("Jumping", true);
                anim.SetBool("Running", false);
                rb2d.AddForce(new Vector2(jumpHorizontalForce*actualDirectionVector, jumpVerticalForce));
                jump = false;
            }

            if (slide)
            {
                slideStart = Time.time;
                anim.SetBool("Running", false);
                anim.SetTrigger("Sliding");
                rb2d.AddForce(new Vector2(slideForce*actualDirectionVector, 0f));
                sliding = true;
                slide = false;
                //SetSlidingTransform();
            }
            if (idleMove && !tryingToJump)
            {

                // advance timer
                idleTimeCounter += Time.deltaTime;

                // if too long idle
                if (idleTimeCounter > idleTimeLimit)
                {
                    timerSpotlight.GetComponent<TimerSpotlight>().CreateSpotlight(tran.position.y);
                }
            }


            if (!idleMove || tryingToJump)
            {
                // if player moves,reset timer
                // reset
                idleTimeCounter = 0;
                timerSpotlight.GetComponent<TimerSpotlight>().TurnBack();
            }
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
