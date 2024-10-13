// This script is adapted from Brackeys' 2D character controller.
// You can find the original script here: https://github.com/Brackeys/2D-Character-Controller

using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	public Animator animator;
	
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[SerializeField] private float m_DashForce = 400f;							// Amount of force added when the player dashes.
	[SerializeField] private float m_DashChargeTime = 1.0f;							// Amount of force added when the player dashes.
	[SerializeField] private float m_DashTimeInAir = 0.2f;
	[SerializeField] private float m_TimeBetweenSwings = 400f;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.

	const float k_GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private float m_Dashable = 100f;            // Time since the player last dashed.
	private float m_Jumpable = 100f;            // Time since the player last dashed.
	private float myGravityScale;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;


	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		myGravityScale = m_Rigidbody2D.gravityScale;
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		m_Dashable += Time.fixedDeltaTime;
		m_Jumpable += Time.fixedDeltaTime;
		m_TimeBetweenSwings += Time.fixedDeltaTime;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded) {
					if (m_Jumpable > 0.2f) {
						animator.Play("Idle");
					}
				}
			}
		}
	}


	public void Move(float move, float jump_time, bool dash, bool attack)
	{
		bool jump = Time.time - jump_time < 0.05f; //Basic input buffering on the jump
		
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}

	
		if (m_Grounded) {
			if (move != 0 && m_Dashable > m_DashChargeTime && m_Jumpable > 0.3f) {
				animator.Play("Run");
			}
			else {
				if (m_Dashable > m_DashChargeTime && m_Jumpable > 0.3f && m_TimeBetweenSwings > 0.2f) {
					animator.Play("Idle");
				}
			}
		}
	
	
		// If the player should jump...
	

		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			m_Jumpable = 0f;
			animator.Play("jump");
			GameObject.Find("/SFXManager/Jump").GetComponent<PitchVariedSFX>().PlaySFX();
			gameObject.GetComponent<PlayerMovement>().ResetJump();
		}
		if (!m_Grounded && m_Dashable > m_DashTimeInAir) {
			animator.Play("jump");
		}
		
		if (m_Dashable > m_DashChargeTime && dash)
		{
			// Add a horizontal force to the player.
			m_Dashable = 0f;
			m_Rigidbody2D.velocity = new Vector2(0f,0f);
			myGravityScale = m_Rigidbody2D.gravityScale;
			m_Rigidbody2D.gravityScale = 0f;
			animator.Play("Dash 2");
			GameObject.Find("/SFXManager/Dash").GetComponent<PitchVariedSFX>().PlaySFX();
			if (m_FacingRight) {
				m_Rigidbody2D.AddForce(new Vector2(m_DashForce, 0f));
			}
			else {	
				m_Rigidbody2D.AddForce(new Vector2(-m_DashForce, 0f));
			}
		}
		if (m_Dashable > m_DashTimeInAir) {
			m_Rigidbody2D.gravityScale = myGravityScale;
		}
		
		if (attack) {
			animator.Play("Attack 2");		
			m_TimeBetweenSwings = 0f;
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	public bool amFacingRight() {
		return m_FacingRight;
	}
}
