using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExperimentalTireBehaviour : MonoBehaviour
{
	[SerializeField] public TrailRenderer trail { get; private set; }
	public Vector2 relativePosition;
	[Header("Steering Settings")]
	public Vector2 steerInput;
	private float steerInputLastFrame;
	public float steerCoefficient;
	public AnimationCurve steerMotorStrengthInputCurve;
	public float steerMotorStrength;
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
	public bool isSliding;


	public Vector2 relativeGroundVelocity { get; private set; }


	void Awake()
	{
		trail = GetComponentInChildren<TrailRenderer>();
		AnimationCurve newCurve = trail.widthCurve;
		Keyframe key = new Keyframe(0, transform.lossyScale.y / .05f);
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
		HandleAcceleration();

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
			relativeGroundVelocity += accelerationInput
							* (accelerationInput > 0 ? frontAcceleration : backAcceleration)
							* accelerationCoefficient
							* Time.fixedDeltaTime * (Vector2)transform.up;
		}
		// void LetAngularVelApproachRelGroundVel()
		// {
		// 	float forceApplied = Vector2.Dot(ForwardVel(-relativeGroundVelocity), transform.up) - angularVel;
		// 	if (isSliding)
		// 	{
		// 		float fric = dynamicFriction / 100 * Time.fixedDeltaTime;
		// 		forceApplied = math.clamp(forceApplied, -fric, fric);
		// 	}
		// 	angularVel += forceApplied;
		// }
	}
	public void HandleSteering(Rigidbody2D carRb)
	{
		float baseAngle = GetBaseAngle(carRb);
		Quaternion desiredRotationClamped = Quaternion.Euler(0, 0, math.clamp(baseAngle, -maxSteerAngle, maxSteerAngle));
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, desiredRotationClamped, maxSteerAngleChange * Time.fixedDeltaTime);

		float GetBaseAngle(Rigidbody2D carRb)
		{
			Vector2 velocity = carRb.GetPointVelocity(transform.position	);
			// Vector2 velocity = carRb.GetPointVelocity(carRb.position + (Vector2)transform.up * transform.localPosition.y);
			if (alignWithVelicity && velocity.magnitude > 12f && Vector2.Dot(velocity, transform.up) > 0)
			{
				float steerForce = steerMotorStrength * steerMotorStrengthInputCurve.Evaluate(accelerationInput);
				float velAngle = -Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg;

				float frictionForce = Vector2.Dot(velocity, carRb.transform.right);
				float t = math.min(0, math.abs(frictionForce) - steerForce);


				Vector2 steerDirection = Vector2.Lerp(carRb.transform.up, velocity.normalized, t);
				float angle = -Mathf.Atan2(steerDirection.x, steerDirection.y) * Mathf.Rad2Deg;


				return angle;
			}
			else
				return 0;
		float desiredAngle = baseAngle + GetSteerInput() * steerCoefficient;
		}
		float GetSteerInput()
		{
			float angle = Mathf.MoveTowardsAngle(steerInputLastFrame, -steerInput.x, maxSteerInputAngleChange * Time.fixedDeltaTime);
			// float angle = -Mathf.Atan2(steerInput.x, steerInput.y) * Mathf.Rad2Deg;

			steerInputLastFrame = angle;
			return angle;
		}
		void HandleHandbreaking()
		{
			if (!isHandBreaking)
				return;
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
