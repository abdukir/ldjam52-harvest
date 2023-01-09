using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : MonoBehaviour
{
	[SerializeField] private GameObject puffPrefab;
	[SerializeField] private GameObject cropPrefab1, cropPrefab2, cropPrefab3;


	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			Transform cropSpawn = collision.transform.GetChild(0);
			Instantiate(puffPrefab, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(null, 0.3f);

			if (cropSpawn.childCount == 0)
			{

				AudioManager.Instance.Play("seedHit");
				if (GameManager.Instance.score < 250)
				{
					//crop lvl1
					if(Random.value < 0.95f)
						Instantiate(cropPrefab1, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(cropSpawn, 0f);
					else
						Instantiate(cropPrefab2, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(cropSpawn, 0f);

				}
				else if (GameManager.Instance.score >= 250 || GameManager.Instance.score < 1000)
				{
					if(Random.value < 0.9f)
						Instantiate(cropPrefab2, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(cropSpawn, 0f);
					else
						Instantiate(cropPrefab3, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(cropSpawn, 0f);
				}
				else if (GameManager.Instance.score >= 1000)
				{
					if (Random.value < 0.7f)
						Instantiate(cropPrefab3, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(cropSpawn, 0f);
					else
						Instantiate(cropPrefab2, cropSpawn.position, Quaternion.identity).ParentSetAndDestroy(cropSpawn, 0f);
				}
			}
			Destroy(gameObject);

		}
	}
}
