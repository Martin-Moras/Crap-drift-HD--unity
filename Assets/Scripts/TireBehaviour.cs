using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.InputSystem;

public class TireBehaviour : MonoBehaviour
{
	[SerializeField] public TrailRenderer trail { get; private set; }
	public Vector2 relativePosition;
	[Header("Steering Settings")]
	public float steerInput = 0;
	public float steerCoefficient;
	public float maxSteerAngle;
	public bool alignWithVelicity;
	[Header("Acceleration Settings")]
	public float accelerationInput;
	public float accelerationCoefficient;
	public float frontAcceleration;
	public float backAcceleration;
	public float maxSpeed;

	[Header("Friction Settings")]
	public bool isHandBreaking = false;
	public float damping;
	public float staticFriction;
	public float dynamicFriction;


	public Vector2 sideVel { get; private set; }
	public Vector2 forwardVel { get; private set; }
	public Vector2 relativeGroundVelocity { get; private set; }


	void Awake()
	{
		trail = GetComponentInChildren<TrailRenderer>();
	}
	void Update()
	{
	}
	private void FixedUpdate()
	{
	}
	public Vector2 GetFrictionForces(Rigidbody2D CarRb)
	{
		SetRelativeGroundVelocity();
		relativeGroundVelocity = -CarRb.GetPointVelocity(relativePosition);
		HandleSteering(CarRb);
		HandleAcceleration();
		SetRelativeVelocity();

		if (!isHandBreaking)
		{
			relativeGroundVelocity += forwardVel;
		}
		if (relativeGroundVelocity.magnitude < staticFriction)
		{
			trail.emitting = false;
			return forwardVel;
		}
		else
		{
			trail.emitting = true;
			return relativeGroundVelocity.normalized * dynamicFriction;
		}

		void HandleAcceleration()
		{
			relativeGroundVelocity += (Vector2)transform.right * accelerationInput *
										(accelerationInput > 0 ?
											frontAcceleration :
											backAcceleration);
			// if (rb.linearVelocity.x + rb.linearVelocity.y < .2f)
			// {
			// 	rb.linearVelocity /= 1.02f;
			// }
		}
		void SetRelativeVelocity()
		{
			forwardVel = transform.right * Vector2.Dot(-relativeGroundVelocity, transform.right);
			sideVel = transform.up * Vector2.Dot(-relativeGroundVelocity, transform.up);
		}
		void SetRelativeGroundVelocity()
		{

		}
	}
	public void HandleSteering(Rigidbody2D carRb)
	{
		float baseAngle;
		Vector2 velocity = carRb.GetPointVelocity(relativePosition);
		if (alignWithVelicity && velocity.magnitude > 1f)
		{
			baseAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
		}
		else
		{
			baseAngle = carRb.rotation;
		}
		float desiredAngle = baseAngle + steerInput * steerCoefficient;
		float angleDifference = Mathf.DeltaAngle(carRb.rotation, desiredAngle);
		if (Mathf.Abs(angleDifference) > maxSteerAngle)
		{
			desiredAngle = carRb.rotation + Mathf.Sign(angleDifference) * maxSteerAngle;
		}
		transform.rotation = Quaternion.Euler(0, 0, desiredAngle);
	}
}
