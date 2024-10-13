using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;
	public Collider2D SwordHitRegion;
	public Collider2D dummy;
	public float runSpeed = 40f;
	public GameObject particles;
	float horizontalMove = 0f;
	float jump_default = -1000000;
	float jump = -1000000;
	bool dash = false;
	bool attack = false;
	
	// Update is called once per frame
	void Update () {
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		if (Input.GetButtonDown("Jump"))
		{
			jump = Time.time;
		}
		if (Input.GetButtonDown("Fire1"))
		{
			//Attack
			attack = true;
			GameObject.Find("/SFXManager/SwordSwing").GetComponent<PitchVariedSFX>().PlaySFX();
			if (SwordHitRegion.IsTouching(dummy)) {
				Debug.Log("Hit");
				GameObject.Find("/SFXManager/DummyHit").GetComponent<PitchVariedSFX>().PlaySFX();
				GameObject myParticles = Instantiate(particles);
				myParticles.transform.position = dummy.transform.position + new Vector3(0, 0, -1);
				myParticles.SetActive(true);
				if (gameObject.GetComponent<CharacterController2D>().amFacingRight()) {
					dummy.GetComponent<Animator>().Play("KnockRight");
				}
				else {
					dummy.GetComponent<Animator>().Play("KnockLeft");
				}
				Destroy(myParticles, 3);
			}
		}
		if (Input.GetButtonDown("Fire2"))
		{
			//Attack (Spawn hitboxy thing)
			dash = true;
		}
	}

	void FixedUpdate ()
	{
		if (gameObject.transform.position.y > -3) {
			// Move our character
			controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash, attack);
			dash = false;
			attack = false;
		}
		else {
			controller.Move(0, jump_default, false, false);
			GameObject.Find("/MusicManager").GetComponent<MusicPlayerSync>().playOnDeath();
			GameObject.Find("/AmbienceManager").GetComponent<MusicPlayerSync>().stopEverything();
		}
	}

	public void ResetJump() {
		jump = jump_default;		
	}
}
