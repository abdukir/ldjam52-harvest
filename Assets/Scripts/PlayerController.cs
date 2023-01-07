using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PlayerState
{
	Gameplay,
	QTA,
	Hold
}

public enum QTA
{
	leftArrow,
	rightArrow,
	upArrow,
	downArrow,
}

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rb;
	private Animator anim;
	[SerializeField] private float moveSpeed = 10f;
	[SerializeField] private float jumpSpeed = 10f;
	private bool facingLeft = true;

	[SerializeField] private float fallMultiplier = 2.5f;
	[SerializeField] private float lowJumpMultiplier = 2f;

	[SerializeField] private float cayoteTime = 0.1f;
	private float cayoteTimeCounter;

	private float jumpBufferTime = 0.2f;
	private float jumpBufferCounter;

	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundCollisionPoint;
	[SerializeField] private float collisionRadius;

	[SerializeField] private Vector3 jumpSquish;
	[SerializeField] private Vector3 landSquish;

	[SerializeField] private bool groundedEnter;
	[SerializeField] private Transform head;

	public PlayerState curState;
	void Start()
	{
		Application.targetFrameRate = 75;
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		curState = PlayerState.Gameplay;
	}

	// Update is called once per frame
	void Update()
	{
		switch (curState)
		{
			case PlayerState.Gameplay:
				UpdateGameplay();
				break;
			case PlayerState.QTA:
				UpdateQTA();
				break;
			case PlayerState.Hold:
				break;
		}

		
	}

	private void UpdateQTA()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
			OnQTAKey(QTA.leftArrow);
		if (Input.GetKeyDown(KeyCode.DownArrow))
			OnQTAKey(QTA.downArrow);
		if (Input.GetKeyDown(KeyCode.RightArrow))
			OnQTAKey(QTA.rightArrow);
		if (Input.GetKeyDown(KeyCode.UpArrow))
			OnQTAKey(QTA.upArrow);
		/*if (Input.GetKeyDown(KeyCode.Space))
			OnQTAKey(QTA.Space);*/
	}

	private void UpdateGameplay()
	{
		Walk();
		Jump();
	}

	private void Walk()
	{
		float x = Input.GetAxis("Horizontal");
		rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

		head.rotation = facingLeft ? Quaternion.Euler(0, 0, -Mathf.Abs(rb.velocity.x)*1.5f) : Quaternion.Euler(0, 0, Mathf.Abs(rb.velocity.x)* 1.5f);

		if (x != 0)
		{
			anim.SetBool("walking", true);
		}
		else
		{
			anim.SetBool("walking", false);
		}

		if (x > 0 && facingLeft)
		{
			Flip();
		}
		if (x < 0 && !facingLeft)
		{
			Flip();
		}
	}

	private void Jump()
	{
		if (IsGrounded())
		{
			cayoteTimeCounter = cayoteTime;
		}
		else
		{
			cayoteTimeCounter -= Time.deltaTime;
		}

		//Actual Jump
		if (jumpBufferCounter > 0 && cayoteTimeCounter > 0f)
		{
			transform.DOPunchScale(jumpSquish, 0.3f, 0, 0).OnComplete(() => transform.localScale = Vector3.one);
			rb.velocity = Vector2.up * jumpSpeed;
			jumpBufferCounter = 0f;
			groundedEnter = true;
			anim.SetBool("jumping", true);
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			jumpBufferCounter = jumpBufferTime;
		}
		else
		{
			jumpBufferCounter -= Time.deltaTime;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			cayoteTimeCounter = 0f;
		}

		if (rb.velocity.y < 0)
		{
			rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		}
		else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.UpArrow))
		{
			rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
		}
	}

	private bool IsGrounded()
	{
		bool grounded = Physics2D.OverlapCircle(groundCollisionPoint.position, collisionRadius, groundLayer);
		if (grounded && groundedEnter)
		{
			transform.DOPunchScale(landSquish, 0.1f, 0, 0).OnComplete(() => transform.localScale = Vector3.one);
			groundedEnter = false;
			anim.SetBool("jumping", false);
		}
		return grounded;
	}

	private void Flip()
	{
		Vector3 currentScale = transform.GetChild(0).localScale;
		currentScale.x *= -1;
		transform.GetChild(0).localScale = currentScale;

		facingLeft = !facingLeft;
	}
	private void OnQTAKey(QTA _key)
	{
		Debug.Log(_key);
	}

}
