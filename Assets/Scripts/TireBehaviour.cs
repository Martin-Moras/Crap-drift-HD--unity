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
	public float staticFriction;
	public float dynamicFriction;
	public float angularVel;


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
		relativeGroundVelocity = -CarRb.GetPointVelocity(transform.position);
		HandleSteering(CarRb);
		if (!isHandBreaking)
		{
			HandleAcceleration();
			relativeGroundVelocity += (Vector2)transform.up * angularVel;
		}
		else	
			angularVel = 0;
			
		if (relativeGroundVelocity.magnitude < staticFriction)
		{
			trail.emitting = false;
			angularVel += Vector2.Dot(ForwardVel(relativeGroundVelocity), transform.up);
			return -ForwardVel(relativeGroundVelocity) - SiedVel(relativeGroundVelocity);
		}
		else
		{
			trail.emitting = true;
			angularVel += math.sign(Vector2.Dot(ForwardVel(relativeGroundVelocity), transform.up)) * dynamicFriction;
			return relativeGroundVelocity.normalized * dynamicFriction;
		}

		void HandleAcceleration()
		{
			angularVel += accelerationInput *
							(accelerationInput > 0 ?
								frontAcceleration :
								backAcceleration);
			// if (angularVel > ForwardVel(relativeGroundVelocity).magnitude + )
		}
		Vector2 ForwardVel(Vector2 groundVel)
		{
			return transform.up * (Vector2.Dot(-relativeGroundVelocity, transform.up));
		}
		Vector2 SiedVel(Vector2 groundVel)
		{
			return transform.right * Vector2.Dot(-relativeGroundVelocity, transform.right);
		}
		void SetRelativeGroundVelocity()
		{

		}
	}
	public void HandleSteering(Rigidbody2D carRb)
	{
		float baseAngle = 0;
		Vector2 velocity = carRb.linearVelocity;//.GetPointVelocity(relativePosition);
		if (alignWithVelicity && velocity.magnitude > 100f)
		{
			float velAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
			baseAngle = Mathf.DeltaAngle(carRb.rotation, velAngle);
		}
		float desiredAngle = baseAngle + steerInput * steerCoefficient * maxSteerAngle;
		desiredAngle = math.clamp(desiredAngle, -maxSteerAngle, maxSteerAngle);

		transform.localRotation = Quaternion.Euler(0, 0, desiredAngle);
	}
}
