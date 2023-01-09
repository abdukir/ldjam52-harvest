using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxFollow : MonoBehaviour
{

    [SerializeField] private Transform target;

    [SerializeField] private Vector2 followDiv;
    [SerializeField] private Vector2 offset;


	private void LateUpdate()
	{
		transform.position = new Vector3(target.position.x / followDiv.x, target.position.y / followDiv.y, 0f) + (Vector3)offset;
	}
}
