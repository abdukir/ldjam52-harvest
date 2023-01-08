using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
	[SerializeField] private float moveSpeed;
	[SerializeField] private Vector2 camFollowDiv;
	[SerializeField] private Camera cam;

	private PlayerController player;
	private void Start()
	{
		player = GameManager.Instance.player;
	}
	private void LateUpdate()
	{
		switch (player.curState)
		{
			case PlayerState.Hold:
				HoldUpdate();
				break;
			case PlayerState.QTA:
				QTAUpdate();
				break;
			case PlayerState.Gameplay:
				GameplayUpdate();
				break;
		}
	}

	private void HoldUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, target.position.ChangeZ(-10), Time.deltaTime * moveSpeed);
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 6f, Time.deltaTime * moveSpeed);
	}

	private void QTAUpdate()
	{
		Vector3 centerPos = GetCenterPoint(player.transform, player.curCrop.transform);
		centerPos.z = -10;
		transform.position = Vector3.Lerp(transform.position, centerPos, Time.deltaTime * moveSpeed);
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,4f,Time.deltaTime * moveSpeed);
	}

	private void GameplayUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x / camFollowDiv.x, target.position.y / camFollowDiv.y, transform.position.z), Time.deltaTime * moveSpeed);
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 7.3f, Time.deltaTime * moveSpeed);
	}

	public Vector3 GetCenterPoint(Transform _target1,Transform _target2)
	{
		var bounds = new Bounds(_target1.position, Vector3.zero);
		bounds.Encapsulate(_target2.position);

		return bounds.center;

	}

}
