using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using CameraShake;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

	public PlayerController player;
	public CrowController crow;

	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private Transform heartParent;

	[SerializeField] public int score;
	public int health = 3;

	public List<Transform> grounds = new List<Transform>();

	public float genMultiplier;
	public float offset;

	private void Awake()
	{
		Instance = this;
	}

	public void AddScore(int _amount)
	{
		score += _amount;
		scoreText.text = score.ToString();
		scoreText.GetComponent<RectTransform>().DOShakePosition(0.1f, 5, 80, 90);
		scoreText.GetComponent<RectTransform>().DOShakeRotation(0.1f, 20, 80, 90);
	}

	public void UpdateHealth(int _amount)
	{
		if (_amount < 0)
		{
			AudioManager.Instance.Play("hurt");
		}
		if (health == 0)
		{
			// Gameover
			Debug.Log("dead");
		}

		health += _amount;
		health = (int)Mathf.Clamp(health, 0,3);
		
		for (int i = 0; i < heartParent.childCount; i++)
		{
			heartParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
			heartParent.GetChild(i).GetComponent<RectTransform>().DOShakeRotation(0.1f, 50, 80, 90);
		}

		for (int i = 0; i < health; i++)
		{
			heartParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
		}
	}

	[ContextMenu("test randomize")]
	public void RandomizeGround()
	{
		CameraShaker.Presets.Explosion2D(3, 3, 3);
		StartCoroutine(player.Fall(false));
		for (int i = 0; i < grounds.Count; i++)
		{
			grounds[i].localPosition = new Vector3(grounds[i].localPosition.x,(Mathf.PerlinNoise(i, grounds[i].position.y)/ genMultiplier) - offset,0f);
		}
	}
}
