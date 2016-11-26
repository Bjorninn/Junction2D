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
        get { return facingRight ? 1 : -1; }
        private set { }
    }

    [HideInInspector]
    public bool doJump = false;
    public float jumpVerticalForce = 1000f;
    public Transform groundCheck;
    public float speed = 20;
    public float slideForce = 100;
    public float slideDuration = 2;
    public GameObject timerSpotlight;

    private bool isGrounded = false;
    private Animator anim;
    private Rigidbody2D rb2d;
    private Transform tran;
    private BoxCollider2D box;
    private AudioSource audio;

    private bool doSlide = false;
    private bool isSliding = false;
    private float slideStart;

    public float deathTimer = 10f;
    private bool isDead = false;
    public float jumpHorizontalForce = 1000f;

    private bool idleMove;
    private bool tryingToJump;
    private float idleTimeLimit;
    private float idleTimeCounter;

    private bool mainMenuEnabled = false;
    private float timescaleDefault;
    private bool isJumping = false;
    private bool isRunning = false;

    public AudioClip reloadingSound;
    public AudioClip shootingSound;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        tran = GetComponent<Transform>();
        box = GetComponent<BoxCollider2D>();
        audio = GetComponent<AudioSource>();

        idleTimeLimit = 5.0f;
        idleTimeCounter = 0.0f;
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

        if (isGrounded && isJumping)
        {
            isJumping = false;
            anim.SetBool("Jumping", false);
        }

        if (v.Equals(0))
        {
            tryingToJump = false;
        }
        else
        {
            tryingToJump = true;
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

    }



    public void Kill()
    {
        if (!isDead)
        {
            isDead = true;
            timerSpotlight.GetComponent<TimerSpotlight>().TurnBack();
            audio.PlayOneShot(reloadingSound, 0.1f);            
            Invoke("_Kill", 1.0f);
        }
    }

    private void _Kill()
    {
        anim.SetTrigger("Death");
        audio.PlayOneShot(shootingSound, 0.5f);
   
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
        else if (isGrounded && !isSliding)
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

        
        if (idleMove && !tryingToJump)
        {
            //	Debug.Log (idleTimeCounter);
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

    private void StopSliding()
    {
        isSliding = false;
        rb2d.velocity = new Vector2(0, rb2d.velocity.y); //we don't want to prevent it from stopping falling
        //SetStandingTransform();
    }

    private void StopRunning()
    {
        isRunning = false;
        anim.SetBool("Running", false);
    }

    private void MoveInPressedDirectionAndFlipIfNecessary(float h)
    {
        tran.Translate(Vector3.right*h*Time.deltaTime*speed);

        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();
        isRunning = true;
        anim.SetBool("Running", true);
    }

    private void Jump()
    {
        anim.SetBool("Jumping", true);
        anim.SetBool("Running", false);
        rb2d.AddForce(new Vector2(jumpHorizontalForce*actualDirectionVector, jumpVerticalForce), ForceMode2D.Impulse);
        doJump = false;
    }

    private void Slide()
    {
        slideStart = Time.time;
        anim.SetBool("Running", false);
        anim.SetTrigger("Sliding");
        rb2d.AddForce(new Vector2(slideForce*actualDirectionVector, 0f));
        isSliding = true;
        doSlide = false;
        //SetSlidingTransform();
    }


    public void Die()
    {

        Destroy(this.gameObject);
        timerSpotlight.GetComponent<TimerSpotlight>().TurnBack();
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
