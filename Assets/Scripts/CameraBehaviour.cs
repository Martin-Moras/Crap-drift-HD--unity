using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
	public Transform target;
	public float size = 60;
	public float maxDistance = 5;
	public float folowRotationSpeed = .5f;
	public Vector3 rotationOffset;
	public Vector3 possitionOffset;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (target == null)
		{
			Debug.Log("Camera Has no target");
			this.enabled = false;
		}
	}

	// Update is called once per frame
	void LateUpdate()
	{
		Quaternion targetRotation = target.rotation * Quaternion.Euler(rotationOffset);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, folowRotationSpeed);

		Vector3 targetPos = target.position + target.rotation * possitionOffset;
		transform.position = targetPos;
	}
}
