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

	private float posStr = 0.05f;
	private float rotStr = 0.1f;
	private float freq = 80f;
	private float numB = 5;

	[SerializeField] private float fallForce;

	[SerializeField] private GameObject puffPrefab;

	public PlayerState curState;

	private AudioManager auM;
	private GameManager gm;

	void Start()
	{
		Application.targetFrameRate = 75;
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		curState = PlayerState.Gameplay;
		auM = AudioManager.Instance;
		gm = GameManager.Instance;
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
			auM.Play("jump");
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



	private IEnumerator Fall(bool _left)
	{
		if (curState == PlayerState.QTA)
		{
			EndQTA();
		}

		anim.enabled = true;
		curState = PlayerState.Hold;
		rb.velocity = Vector2.zero;
		rb.sharedMaterial.bounciness = 0.5f;
		rb.sharedMaterial.friction = 0.1f;
		
		GetComponent<Collider2D>().enabled = false;
		GetComponent<Collider2D>().enabled = true;

		anim.SetBool("fall", true);
		rb.velocity = (_left ? new Vector2(-1, 2) : new Vector2(1, 2)) * fallForce;
		//rb.AddForce((_left ? new Vector2(-1, 2) : new Vector2(1, 2)) * Time.deltaTime * fallForce, ForceMode2D.Impulse);

		yield return new WaitForSeconds(1.5f);

		curState = PlayerState.Gameplay;
		rb.sharedMaterial.bounciness = 0;
		rb.sharedMaterial.friction = 0f;

		GetComponent<Collider2D>().enabled = false;
		GetComponent<Collider2D>().enabled = true;

		anim.SetBool("fall", false);

		yield return null;
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

		if (_key == QTAS[0].qtaType)
		{
			CameraShaker.Presets.ShortShake2D(posStr, rotStr, freq, (int)numB);
			// Correct qta
			QTAS[0].gameObject.SetActive(false);
			PullAnim(_key);
			lastKey = _key;
			QTAS.RemoveAt(0);
			if (Random.value < 0.5f)
				auM.Play("qta1");
			else
				auM.Play("qta2");
		}
		else
		{
			Instantiate(puffPrefab, curCrop.transform.position, Quaternion.identity).ParentSetAndDestroy(null, 0.3f);
			// Wrong QTA
			auM.Play("wrongqta");
			if (Extensions.AngleDir((Vector2)transform.position,(Vector2)curCrop.transform.position) > 0)
			{
				EndQTA();
				StartCoroutine(Fall(false));
				curCrop = null;
				
				return;
			}
			else
			{
				EndQTA();
				StartCoroutine(Fall(true));
				curCrop = null;
				return;
			}
		}

		if (QTAS.Count == 0)
		{
			//End event
			EndQTA();
			EndQtaAnim(curCrop.transform);
			gm.AddScore(curCrop.score);
			curCrop.GetComponent<BoxCollider2D>().enabled = false;
			return;
		}

	}

	private void EndQtaAnim(Transform _crop)
	{
		Sequence endSeq = DOTween.Sequence();
		_crop.SetParent(null);
		auM.Play("pickup");
		switch (lastKey)
		{
			case QTA.leftArrow:
				endSeq.Append(_crop.DOJump(leftBucket.position, 9, 1, 2f));
				endSeq.Join(_crop.DORotate(new Vector3(0, 0, 1500), 3, RotateMode.FastBeyond360));
				break;
			case QTA.rightArrow:
				endSeq.Join(_crop.DOJump(rightBucket.position, 9, 1, 2f));
				endSeq.Join(_crop.DORotate(new Vector3(0, 0, 1500), 3, RotateMode.FastBeyond360));
				break;
			case QTA.upArrow:
				endSeq.Append(_crop.DOJump((Random.value > 0.5f) ? leftBucket.position : rightBucket.position, 9, 1, 2f));
				endSeq.Join(_crop.DORotate(new Vector3(0, 0, 1500), 3, RotateMode.FastBeyond360));
				break;
			case QTA.downArrow:
				endSeq.Append(_crop.DOJump((Random.value > 0.5f) ? leftBucket.position : rightBucket.position, 9, 1, 2f));
				endSeq.Join(_crop.DORotate(new Vector3(0, 0, 1500), 3, RotateMode.FastBeyond360));
				break;
		}
		endSeq.OnComplete(() => Destroy(_crop.gameObject));
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
				curCrop.transform.GetChild(0).DOPunchPosition(Vector3.up, 0.1f, 1);
				break;
			case QTA.downArrow:
				curCrop.transform.GetChild(0).DOPunchPosition(Vector3.down, 0.1f, 1);
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
				if (curState == PlayerState.Gameplay)
				{
					curCrop = collision.GetComponent<Crop>();
					StartQTA();
				}
				else if (curState == PlayerState.Hold)
				{
					Destroy(collision.gameObject);
					Instantiate(puffPrefab, collision.transform.position, Quaternion.identity).ParentSetAndDestroy(null, 0.3f);
				}
				break;

			case "Heart":
				gm.UpdateHealth(1);
				auM.Play("heart");
				Instantiate(puffPrefab, collision.transform.position, Quaternion.identity).ParentSetAndDestroy(null, 0.3f);
				Destroy(collision.gameObject);
				break;

			case "Crow":
				if (curState == PlayerState.Hold) return;
				if (gm.crow.curState == CrowState.Attacking)
				{
					StartCoroutine(Fall(false));
					gm.UpdateHealth(-1);
					gm.crow.curState = CrowState.Patroling;
				}else if (gm.crow.curState == CrowState.Patroling)
				{
					StartCoroutine(Fall(false));
					gm.UpdateHealth(-1);
				}
				break;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		switch (collision.tag)
		{
			case "Crop":
				if (curState == PlayerState.QTA)
				{
					curCrop = null;
					EndQTA();
				}
				break;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Rock"))
		{
			if (collision.gameObject.GetComponent<Rock>().isActive)
			{
				StartCoroutine(Fall((Extensions.AngleDir(collision.contacts[0].point,transform.position) < 0)));
				gm.UpdateHealth(-1);
				Destroy(collision.gameObject);
			}
		}
	}

	private void EndQTA()
	{
		curState = PlayerState.Gameplay;
		gm.crow.curState = CrowState.Patroling;
		anim.enabled = true;
		rb.drag = 0f;
		QTAIconHolder.gameObject.SetActive(false);
		QTAS.Clear();
	}


	private void StartQTA()
	{
		curState = PlayerState.QTA;
		gm.crow.curState = CrowState.Attacking;

		rb.velocity= Vector3.zero;
		rb.drag = 10f;
		anim.SetBool("jumping", false);
		anim.SetBool("walking", false);
		anim.enabled = false;

		arm.DOLocalRotate(new Vector3(0, 0, 90), 0.2f);
		for (int i = 0; i < QTAIconHolder.childCount; i++)
		{
			QTAIconHolder.GetChild(i).gameObject.SetActive(false);
		}
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
