using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraShake;
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
	[SerializeField] private Transform arm;

	[SerializeField] private Transform leftBucket,rightBucket;

	[SerializeField] private Transform QTAIconHolder;
	[SerializeField] private GameObject QTAIconPrefab;

	[SerializeField] private List<QTAIcon> QTAS = new List<QTAIcon>();

	private QTA lastKey;

	public Crop curCrop;

	public float posStr, rotStr, freq, numB;

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
		if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.A))
			OnQTAKey(QTA.leftArrow);
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
			OnQTAKey(QTA.downArrow);
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
			OnQTAKey(QTA.rightArrow);
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
			OnQTAKey(QTA.upArrow);
		rb.velocity.ChangeY2D(0);
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

		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			jumpBufferCounter = jumpBufferTime;
		}
		else
		{
			jumpBufferCounter -= Time.deltaTime;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
		{
			cayoteTimeCounter = 0f;
		}

		if (rb.velocity.y < 0)
		{
			rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		}
		else if (rb.velocity.y > 0 && !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)))
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
		CameraShaker.Presets.ShortShake2D(posStr, rotStr, freq, (int)numB);

		if (_key == QTAS[0].qtaType)
		{
			// Correct qta
			QTAS[0].gameObject.SetActive(false);
			PullAnim(_key);
			lastKey = _key;
			QTAS.RemoveAt(0);
		}

		if (QTAS.Count == 0)
		{
			//End event
			EndQTA();
			EndQtaAnim();
			return;
		}
		Debug.Log(_key);
	}

	private void EndQtaAnim()
	{
		Sequence endSeq = DOTween.Sequence();
		switch (lastKey)
		{
			case QTA.leftArrow:
				endSeq.Append(curCrop.transform.DOJump(leftBucket.position, 9, 1, 2f));
				endSeq.Join(curCrop.transform.DORotate(new Vector3(0, 0, 1500), 3, RotateMode.FastBeyond360));
				break;
			case QTA.rightArrow:
				endSeq.Join(curCrop.transform.DOJump(rightBucket.position, 9, 1, 2f));
				endSeq.Join(curCrop.transform.DORotate(new Vector3(0, 0, 1500), 3, RotateMode.FastBeyond360));
				break;
			case QTA.upArrow:
				endSeq.Append(curCrop.transform.DOJump((Random.value > 0.5f) ? leftBucket.position : rightBucket.position, 9, 1, 2f));
				endSeq.Join(curCrop.transform.DORotate(new Vector3(0, 0, 1500), 3, RotateMode.FastBeyond360));
				break;
			case QTA.downArrow:
				endSeq.Append(curCrop.transform.DOJump((Random.value > 0.5f) ? leftBucket.position : rightBucket.position, 9, 1, 2f));
				endSeq.Join(curCrop.transform.DORotate(new Vector3(0, 0, 1500), 3, RotateMode.FastBeyond360));
				break;
		}

	}

	private void PullAnim(QTA _key)
	{
		curCrop.transform.GetChild(0).DOShakePosition(0.1f, 0.3f, 50, 90);
		curCrop.transform.GetChild(0).DOShakeRotation(0.1f, 30, 50, 90);
		switch (_key)
		{
			case QTA.leftArrow:
				LeftRightAnim(curCrop.transform.GetChild(0), true);
				break;
			case QTA.rightArrow:
				LeftRightAnim(curCrop.transform.GetChild(0), false);
				break;
			case QTA.upArrow:
				break;
			case QTA.downArrow:
				break;
			default:
				break;
		}
	}

	private void LeftRightAnim(Transform _crop, bool _isLeft)
	{
		float rotAmount = 10f;
		if (_crop.eulerAngles.z < -15f || _crop.eulerAngles.z > 15f)
		{
			rotAmount = 0f;
		}
		_crop.DOLocalRotate(new Vector3(0, 0, _crop.transform.eulerAngles.z + (_isLeft ? rotAmount : -rotAmount)), 0.1f);
		arm.DOLocalMoveX((_isLeft ? -0.3f : 0.2f), 0.1f);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.tag)
		{
			case "Crop":
				curCrop = collision.GetComponent<Crop>();
				StartQTA();
				break;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		switch (collision.tag)
		{
			case "Crop":
				curCrop = null;
				EndQTA();
				break;
		}
	}

	private void EndQTA()
	{
		curState = PlayerState.Gameplay;
		anim.enabled = true;
		QTAIconHolder.gameObject.SetActive(false);
		QTAS.Clear();

	}


	private void StartQTA()
	{
		curState = PlayerState.QTA;
		rb.velocity= Vector3.zero;
		anim.SetBool("jumping", false);
		anim.SetBool("walking", false);
		anim.enabled = false;

		arm.DOLocalRotate(new Vector3(0, 0, 90), 0.2f);
		for (int i = 0; i < curCrop.QTAS.Count; i++)
		{
			//QTAS.Add(Instantiate(QTAIconPrefab, QTAIconHolder).GetComponent<QTAIcon>());
			QTAS.Add(QTAIconHolder.GetChild(i).GetComponent<QTAIcon>());
			QTAS[i].gameObject.SetActive(true);
			QTAS[i].qtaType = curCrop.QTAS[i];
			QTAS[i].UpdateIcon();
		}
		QTAIconHolder.gameObject.SetActive(true);
	}
}
