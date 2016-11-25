﻿using UnityEngine;
using System.Collections;

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
	public float jumpForce = 1000f;
	public Transform groundCheck;
	public float speed = 20;
	public float slideForce = 100;
	public float slideDuration = 2;


	private bool grounded = false;
	private Animator anim;
	private Rigidbody2D rb2d;
	private Transform tran;
	private BoxCollider2D box;
	private bool slide = false;
	private bool sliding = false;
	private float slideStart;


	// Use this for initialization
	void Awake()
	{
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		tran = GetComponent<Transform>();
		box = GetComponent<BoxCollider2D>();
	}

	// Update is called once per frame
	void Update()
	{
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		var v = Input.GetAxis("Vertical");
		if (v > 0 && grounded)
		{
			jump = true;
		}
		else if (v < 0 && grounded && !sliding)
		{
			slide = true;
		}
	}

	void FixedUpdate()
	{
		if (!sliding)
		{
			float h = Input.GetAxis("Horizontal");

			anim.SetFloat("Speed", Mathf.Abs(h));

			tran.Translate(Vector3.right*h*Time.deltaTime*speed);

			if (h > 0 && !facingRight)
				Flip();
			else if (h < 0 && facingRight)
				Flip();
		}
		else if(Time.time - slideStart >= slideDuration)
		{
			sliding = false;
			rb2d.velocity = Vector2.zero;
			SetStandingTransform();

		}

		if (jump)
		{
			anim.SetTrigger("Jump");
			rb2d.AddForce(new Vector2(0f, jumpForce));
			jump = false;
		}

		if (slide)
		{
			slideStart = Time.time;
			anim.SetTrigger("Slide");
			rb2d.AddForce(new Vector2(slideForce * actualDirectionVector, 0f));
			sliding = true;
			slide = false;
			SetSlidingTransform();
		}
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
