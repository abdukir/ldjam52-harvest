using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System.Linq;

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
	[SerializeField] private CrowState curState;

	private Rigidbody2D rb;
	[SerializeField] private float moveSpeed;
	private bool facingLeft;

	[SerializeField] private GameObject rockPrefab, seedPrefab;

	private void Start()
	{
		rb= GetComponent<Rigidbody2D>();
		curState = CrowState.Patroling;
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

				break;
			case CrowState.Fleeing:
				break;
			case CrowState.Hold:

				break;
		}
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

		if (Extensions.AngleDir(transform.position, waypoints[0]) > 0 && facingLeft)
		{
			Flip();
		}
		if (Extensions.AngleDir(transform.position, waypoints[0]) < 0 && !facingLeft)
		{
			Flip();
		}
	}
	private void Flip()
	{
		Vector3 currentScale = transform.localScale;
		currentScale.x *= -1;
		transform.localScale = currentScale;

		facingLeft = !facingLeft;
	}


	private void Drop()
	{
		if (Random.value < 0.7f)
		{
			//Drop
			if (Random.value < 0.2f)
			{
				//Drop Rock
				Instantiate(rockPrefab,transform.GetChild(0).position,Quaternion.identity).ParentSetAndDestroy(null,3f);
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
