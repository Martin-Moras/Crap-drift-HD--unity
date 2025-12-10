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
	public float steerInput;
	private float steerInputLastFrame;
	public float steerCoefficient;
	public float maxSteerAngle;
	public float maxSteerAngleChange;
	public float maxSteerInputAngleChange;
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
	public Vector2 GetFrictionForces(Rigidbody2D carRb)
	{
		relativeGroundVelocity = -carRb.GetPointVelocity(transform.position);
		HandleSteering(carRb);
		LetAngularVelApproachRelGroundVel();
		HandleAcceleration();
		HandleHandbreaking();
		relativeGroundVelocity += (Vector2)transform.up * angularVel;

		if (isSliding && relativeGroundVelocity.magnitude < stopSlidingVelocity)
			isSliding = false;
		if (relativeGroundVelocity.magnitude < staticFriction && !isSliding)
		{
			trail.emitting = false;
			return (-ForwardVel(-relativeGroundVelocity) - SiedVel(-relativeGroundVelocity)) * carRb.mass / Time.fixedDeltaTime;
		}
		else
		{
			isSliding = true;
			trail.emitting = true;
			return relativeGroundVelocity.normalized * math.clamp(dynamicFriction, 0, relativeGroundVelocity.magnitude * carRb.mass / Time.fixedDeltaTime);
		}

		void HandleAcceleration()
		{
			angularVel += accelerationInput
							* (accelerationInput > 0 ? frontAcceleration : backAcceleration)
							* accelerationCoefficient * accelerationCurve.Evaluate(math.abs(angularVel))
							* Time.fixedDeltaTime;
		}
		void LetAngularVelApproachRelGroundVel()
		{
			float forceApplied = Vector2.Dot(ForwardVel(-relativeGroundVelocity), transform.up) - angularVel;
			if (isSliding)
			{
				float fric = dynamicFriction / 200 * Time.fixedDeltaTime;
				forceApplied = math.clamp(forceApplied, -fric, fric);
			}
			angularVel += forceApplied;
		}
		void HandleHandbreaking()
		{
			if (!isHandBreaking)
				return;
			var breakForce = handBreakForce * math.sign(angularVel) * Time.fixedDeltaTime;
			var clamped = math.clamp(breakForce, -math.abs(angularVel), math.abs(angularVel));
			angularVel -= clamped;
		}
	}
	public void HandleSteering(Rigidbody2D carRb)
	{
		float baseAngle = GetBaseAngle(carRb);
		float desiredAngle = baseAngle + GetSteerInput() * steerCoefficient;
		Quaternion desiredRotationClamped = Quaternion.Euler(0, 0, math.clamp(desiredAngle, -maxSteerAngle, maxSteerAngle));
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, desiredRotationClamped, maxSteerAngleChange);

		float GetBaseAngle(Rigidbody2D carRb)
		{
			Vector2 velocity = carRb.linearVelocity;
			if (alignWithVelicity && velocity.magnitude > 10f && Vector2.Dot(velocity, transform.up) > 0)
			{
				float velAngle = -Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg;

				return Mathf.DeltaAngle(carRb.rotation, velAngle);
			}
			else
				return 0;
		}
		float GetSteerInput()
		{
			float angle = Mathf.MoveTowardsAngle(steerInputLastFrame, steerInput, maxSteerInputAngleChange);
			steerInputLastFrame = angle;
			return angle;
		}
	}
	Vector2 ForwardVel(Vector2 velocity)
	{
		return transform.up * Vector2.Dot(velocity, transform.up);
	}
	Vector2 SiedVel(Vector2 velocity)
	{
		return transform.right * Vector2.Dot(velocity, transform.right);
	}
}
