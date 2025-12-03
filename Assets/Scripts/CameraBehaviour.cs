using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
	public Transform target;
	public CinemachineCamera cinemachineCamera;
	public float size = 60;
	public float maxDistance = 5;
	public float folowRotationSpeed = .5f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		cinemachineCamera = GetComponent<CinemachineCamera>();
		target = cinemachineCamera.Target.TrackingTarget;
		if (target == null)
		{
			Debug.Log("Camera Has no target");
			this.enabled = false;
		}
	}

	// Update is called once per frame
	void LateUpdate()
	{
		Quaternion targetRotation = target.rotation;
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, folowRotationSpeed);
	}
}
