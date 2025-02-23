using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class backTires : MonoBehaviour
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

	//local
	bool isHandBreaking = false;


	void Start()
	{

	}
	void Update()
	{
		if (Input.GetKey(KeyCode.Space)) isHandBreaking = true;
		else isHandBreaking = false;
	}
	private void FixedUpdate()
	{
		move();
	}
	void move()
	{
		float newHandbreakStrength = 0;
		float newStability = stability;
		Debug.Log(Gamepad.all[0].leftShoulder.scaleFactor);
		if (Input.GetKey(KeyCode.Mouse1))
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
		}

		if (isHandBreaking)
		{
			newHandbreakStrength = HandbreakStrength;
			newStability = 0f;
		}
		
		Vector2 forwardVel = transform.right * Vector2.Dot(rb.velocity, transform.right);
		Vector2 sideVel = transform.up * Vector2.Dot(rb.velocity, transform.up);
		
		//test.transform.position = transform.position + (transform.up * -friction);
		
		rb.AddForce(-forwardVel * newHandbreakStrength);
		
		if (sideVel.magnitude < newStability)
		{
			rb.velocity = forwardVel;
			trail.emitting = false;
		}
		else
		{
			rb.AddForce(sideVel / sideVel.magnitude * (-friction + sideVel.magnitude + newHandbreakStrength));
			trail.emitting = true;
		}
		
		
		//Debug.Log(transform.up);
	}
}
