using Unity.Mathematics;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
	public Rigidbody2D target;
	public Camera camera;
	public float size = 60;
	public float folowSpeed = 1;
	public float maxDistance = 5;
	public float folowRotationSpeed = .5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		if (target == null)
		{
			Debug.Log("Camera Has no target");
			this.enabled = false;
		}
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
		camera.orthographicSize = size;
        Vector2 targetPos = target.position + (Vector2)math.clamp(target.linearVelocity, 0, maxDistance);
		transform.position = (Vector3)Vector2.Lerp(transform.position, targetPos, folowSpeed) + Vector3.back * 10f;

		float targetRotation = target.rotation;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,targetRotation), folowRotationSpeed);
    }
}
