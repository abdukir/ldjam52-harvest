using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
	[SerializeField] private float moveSpeed;
	[SerializeField] private Vector2 camFollowDiv;
	private void LateUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x / camFollowDiv.x, target.position.y / camFollowDiv.y, transform.position.z),Time.deltaTime * moveSpeed);
	}

}
