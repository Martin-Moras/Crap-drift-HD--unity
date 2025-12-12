using Unity.Cinemachine;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
	public Transform target;
	public float size = 60;
	public float maxDistance = 5;
	public float folowRotationSpeed = .5f;
	[Range(0, 1)]
	public float folowSpeed = .5f;
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
	void FixedUpdate()
	{
		Vector3 targetPos = target.position + target.rotation * possitionOffset;// + (Vector3)(-target.GetComponent<Rigidbody2D>().linearVelocity.normalized * size);
		transform.position = targetPos;// Vector3.Lerp(transform.position, targetPos, (transform.position - targetPos).magnitude * folowSpeed * Time.deltaTime);

		Quaternion targetRotation = target.rotation * Quaternion.Euler(rotationOffset);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, folowRotationSpeed * Time.deltaTime);
		// transform.LookAt(new Vector3(target.position.x + rotationOffset.x, target.position.y + rotationOffset.y, rotationOffset.z), Vector3.back);
	}
}
