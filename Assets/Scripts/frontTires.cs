using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frontTires : MonoBehaviour
{
	[SerializeField] Rigidbody2D rb;
	[SerializeField] TrailRenderer trail;
	[SerializeField] HingeJoint2D joint;
	[SerializeField] float speed;
	[SerializeField] float backSpeed;
	[SerializeField] float damping;
	[SerializeField] float stability;
	[SerializeField] float HandbreakStrength;
	[SerializeField] float friction;
	[SerializeField] float rotationSpeed;

	//local
	bool isHandBreaking = false;


	void Start()
	{
		
	}
	void Update()
	{
		if (Input.GetKey(KeyCode.Space)) isHandBreaking = true;
		else isHandBreaking = false;
		lookAtMouse();
	}
	private void FixedUpdate()
	{
		move();
	}

	void lookAtMouse()
	{
		Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		float newRotationSpeed = rotationSpeed;
		//if (isHandBreaking) newRotationSpeed = .06f;
		//if (joint.jointAngle > 360) 
		//if (joint.jointAngle <= 60 && joint.jointAngle >= -60)
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotationSpeed);
		Debug.Log(joint.jointAngle);
	}
	void move()
	{
		float newHandbreakStrength = 0;
		float newStability = stability;

		/*if (Input.GetKey(KeyCode.Mouse1))
		{
			rb.AddForce(transform.right * -Mathf.Pow(1 - damping, backSpeed * Time.deltaTime));
		}
		else if (Input.GetKey(KeyCode.Mouse0))
		{
			rb.AddForce(transform.right * Mathf.Pow(1 - damping, speed * Time.deltaTime));
		}
		else if (rb.velocity.x + rb.velocity.y < .2f)
		{
			rb.velocity /= 1.02f;
		}*/

		/*if (isHandBreaking)
		{
			newHandbreakStrength = HandbreakStrength;
			newStability = 0f;
		}*/

		Vector2 forwardVel = transform.right * Vector2.Dot(rb.velocity, transform.right);
		Vector2 sideVel = transform.up * Vector2.Dot(rb.velocity, transform.up);

		//test.transform.position = transform.position + (transform.up * -friction);
		Debug.Log(sideVel);
		rb.AddForce(-forwardVel * newHandbreakStrength);
		
		if (sideVel.magnitude < newStability)
		{
			rb.velocity = forwardVel;
			trail.emitting = false;
		}
		else
		{
			rb.AddForce(sideVel / sideVel.magnitude * (-friction + (sideVel.magnitude / 3)));
			trail.emitting = true;
		}
		
		
		//Debug.Log(transform.up);
	}
}
