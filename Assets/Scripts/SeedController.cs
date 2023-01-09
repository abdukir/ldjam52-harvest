using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : MonoBehaviour
{
	[SerializeField] private GameObject puffPrefab;
	[SerializeField] private GameObject cropPrefab;


	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			Transform cropSpawn = collision.transform.GetChild(0);
			Instantiate(puffPrefab, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(null, 0.3f);

			if (cropSpawn.childCount == 0)
			{
				GameObject crop = Instantiate(cropPrefab, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(cropSpawn, 0f);
			}
			Destroy(gameObject);

		}
	}
}
