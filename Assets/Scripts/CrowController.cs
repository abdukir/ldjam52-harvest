using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public enum CrowState
{
	Patroling,
	Attacking,
	Fleeing,
	Hold
}
public class CrowController : MonoBehaviour
{
	[SerializeField] private Vector2 randPointWidth;

	[SerializeField] private Vector2 randPointHeight;

	[SerializeField] private int waypointAmount;
	private List<Vector3> waypoints = new List<Vector3>();
	[SerializeField] public CrowState curState;

	private Rigidbody2D rb;
	[SerializeField] private float moveSpeed;
	private bool facingLeft;

	[SerializeField] private GameObject rockPrefab, seedPrefab, heartPrefab;
	private AudioManager auM;
	private GameManager gm;
	private Vector2 lastPos;
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		curState = CrowState.Patroling;
		auM = AudioManager.Instance;
		gm = GameManager.Instance;
		GenerateWaypoints();
	}

	[ContextMenu("TestGenWaypoints")]
	public void GenerateWaypoints()
	{
		waypoints.Clear();
		for (int i = 0; i < waypointAmount; i++)
		{
			float randHeight = Random.Range(randPointHeight.x, randPointHeight.y);
			if (i == 0)
			{
				waypoints.Add(new Vector3(-14f, randHeight,0f));
			}
			else
			{
				float randWidth = waypoints[i - 1].x + Random.Range(randPointWidth.x, randPointWidth.y);
				waypoints.Add(new Vector3(randWidth, randHeight, 0f));
			}

		}
		waypoints = waypoints.OrderBy(x => Vector3.Distance(transform.position,x)).ToList();
	}

	private void Update()
	{
		switch (curState)
		{
			case CrowState.Patroling:
				UpdatePatrol();
				break;
			case CrowState.Attacking:
				UpdateAttack();
				break;
			case CrowState.Fleeing:
				break;
			case CrowState.Hold:

				break;
		}
		if (gm.player.curCrop != null)
		{
			switch (gm.player.curCrop.cropLevel)
			{
				case Crop.CropLevel.Level1:
					moveSpeed = 5f;
					break;
				case Crop.CropLevel.Level2:
					moveSpeed = 4f;
					break;
				case Crop.CropLevel.Level3:
					moveSpeed = 3f;
					break;
			}
		}
		else
		{
			moveSpeed = 5f;
		}
		
	}

	private void UpdateAttack()
	{
		Vector2 newPosition = Vector2.MoveTowards(transform.position, (Vector2)gm.player.transform.position, Time.deltaTime * moveSpeed);
		rb.MovePosition(newPosition);

		if (transform.position.x > gm.player.transform.position.x)
		{
			transform.localScale = new Vector3(-1, 1, 1);
		}
		if (transform.position.x < gm.player.transform.position.x)
		{
			transform.localScale = new Vector3(1, 1, 1);
		}
		Debug.Log(Extensions.AngleDir(transform.position, gm.player.transform.position));
	}

	private void UpdatePatrol()
	{
		if (Vector2.Distance((Vector2)transform.position, (Vector2)waypoints[0]) < 0.001f)
		{
			//Next
			waypoints.RemoveAt(0);
			// Drop Seed or Rock
			Drop();
			if (waypoints.Count == 0)
			{
				GenerateWaypoints();
			}

		}
		else
		{
			Vector2 newPosition = Vector2.MoveTowards(transform.position, (Vector2)waypoints[0], Time.deltaTime * moveSpeed);
			rb.MovePosition(newPosition);
		}

		if (transform.position.x > waypoints[0].x)
		{
			transform.localScale = new Vector3(-1, 1, 1);
		}
		if (transform.position.x < waypoints[0].x)
		{
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

	private void Drop()
	{
		if (Random.value < 0.8f)
		{
			//Drop
			auM.Play("crowSeedDrop");
			float rand = Random.value;
			if (rand < 0.2f)
			{
				//Drop Rock
				Instantiate(rockPrefab,transform.GetChild(0).position,Quaternion.identity).ParentSetAndDestroy(null,3f);
			}else if (rand < (gm.health < 1 ? 0.1f : 0.3f))
			{
				Instantiate(heartPrefab, transform.GetChild(0).position, Quaternion.identity);
			}
			else
			{
				// Drop seed
				Instantiate(seedPrefab, transform.GetChild(0).position, Quaternion.identity);

			}
		}

	}

	private void OnDrawGizmos()
	{
		foreach (var point in waypoints)
		{
			Gizmos.DrawSphere(point, 1f);	
		}
	}

}
