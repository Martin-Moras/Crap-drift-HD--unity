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
	public AnimationCurve accelerationCurve;
	public float accelerationInput;
	public float accelerationCoefficient;
	public float frontAcceleration;
	public float backAcceleration;
	public float maxSpeed;
	public bool isHandBreaking = false;
	public float handBreakForce;

	[Header("Friction Settings")]
	public float staticFriction;
	public float stopSlidingVelocity;
	public float dynamicFriction;
	public float angularVel;
	public bool isSliding;


	public Vector2 relativeGroundVelocity { get; private set; }


	void Awake()
	{
		trail = GetComponentInChildren<TrailRenderer>();
		AnimationCurve newCurve = trail.widthCurve;
		Keyframe key = new Keyframe(0, transform.lossyScale.y);
		newCurve.MoveKey(0, key);
		trail.widthCurve = newCurve;
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
		HandleAcceleration();
		if (isHandBreaking)
		{
			angularVel -= math.clamp(handBreakForce * math.sign(angularVel) * Time.fixedDeltaTime, -angularVel, angularVel);
		}
		relativeGroundVelocity += (Vector2)transform.up * angularVel;

		if (isSliding && relativeGroundVelocity.magnitude < stopSlidingVelocity)
			isSliding = false;
		if (relativeGroundVelocity.magnitude < staticFriction && !isSliding)
		{
			trail.emitting = false;
			angularVel += Vector2.Dot(ForwardVel(-relativeGroundVelocity), transform.up);
			return -ForwardVel(-relativeGroundVelocity) - SiedVel(-relativeGroundVelocity);
		}
		else
		{
			isSliding = true;
			trail.emitting = true;
			angularVel += math.sign(Vector2.Dot(ForwardVel(-relativeGroundVelocity), transform.up)) * dynamicFriction;
			return relativeGroundVelocity.normalized * dynamicFriction;
		}

		void HandleAcceleration()
		{
			angularVel += accelerationInput
							* (accelerationInput > 0 ? frontAcceleration : backAcceleration)
							* accelerationCoefficient * accelerationCurve.Evaluate(math.abs(angularVel));
		}
		void SetRelativeGroundVelocity()
		{

		}
	}
	public void HandleSteering(Rigidbody2D carRb)
	{
		float baseAngle = 0;
		Vector2 velocity = carRb.linearVelocity;//.GetPointVelocity(relativePosition);
		if (alignWithVelicity && velocity.magnitude > 10f && Vector2.Dot(velocity, transform.up) > 0)
		{
			// Debug.DrawLine(transform.position, (Vector2)transform.position + velocity.normalized * 1000, Color.blue);
			float velAngle = -Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg;
			// Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0,0, velAngle) * Vector2.up * 100, Color.lightBlue);

			baseAngle = Mathf.DeltaAngle(carRb.rotation, velAngle);
			// Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0,0, carRb.transform.rotation.eulerAngles.z + baseAngle) * Vector2.up * 100, Color.green);
		}
		float desiredAngle = baseAngle + steerInput * steerCoefficient;
		desiredAngle = math.clamp(desiredAngle, -maxSteerAngle, maxSteerAngle);

		transform.localRotation = Quaternion.Euler(0, 0, desiredAngle);
	}
	Vector2 ForwardVel(Vector2 velocity)
	{
		return transform.up * (Vector2.Dot(velocity, transform.up));
	}
	Vector2 SiedVel(Vector2 velocity)
	{
		return transform.right * Vector2.Dot(velocity, transform.right);
	}
}
