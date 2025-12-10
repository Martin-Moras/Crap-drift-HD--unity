using System.Collections;
using System.Collections.Generic;
using MertStudio.Car.Sounds;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	private struct ForcePosition
	{

		public Vector2 pos;
		public Vector2 force;
		public ForcePosition(Vector2 force, Vector2 pos)
		{
			this.force = force;
			this.pos = pos;
		}
	}
	public Transform centerOfMass;
	public Rigidbody2D rb { get; private set; }
	private InputAction steeringInput;
	private InputAction accelerationInput;
	private InputAction handBreakBackInput;
	private InputAction handBreakFrontInput;
	private CarAudioController carAudioController;
	public List<TireBehaviour> tires;
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		carAudioController = GetComponentInChildren<CarAudioController>();
		SetCenterOfMassObj();
		rb.centerOfMass = centerOfMass.localPosition;

		steeringInput = InputSystem.actions.FindAction("Steer");
		accelerationInput = InputSystem.actions.FindAction("Acceleration");
		handBreakBackInput = InputSystem.actions.FindAction("HandbreakBack");
		handBreakFrontInput = InputSystem.actions.FindAction("HandbreakFront");

		foreach (TireBehaviour tire in tires)
		{
			tire.relativePosition = tire.transform.localPosition;
		}
	}
	void Update()
	{
		rb.centerOfMass = centerOfMass.localPosition;
	}
	private void FixedUpdate()
	{
		ManageTires();
		HandleAudio();
	}
	private void ManageTires()
	{
		ForcePosition[] forcePositions = new ForcePosition[tires.Count];
		for (int i = 0; i < tires.Count; i++)
		{
			TireBehaviour tire = tires[i];

			tire.steerInput = -steeringInput.ReadValue<float>();
			tire.accelerationInput = accelerationInput.ReadValue<float>();
			tire.isHandBreaking = tire.relativePosition.y > 0 ? handBreakFrontInput.IsPressed() : handBreakBackInput.IsPressed();

			Vector2 frictionForce = tire.GetFrictionForces(rb);
			forcePositions[i] = new ForcePosition(frictionForce, tire.transform.position);
			float visLineReducer = 5000;
			Debug.DrawLine(tire.transform.position, (Vector2)tire.transform.position + frictionForce.normalized * tire.staticFriction / Time.fixedDeltaTime * rb.mass / visLineReducer, Color.red);
			Debug.DrawLine(tire.transform.position, (Vector2)tire.transform.position + frictionForce / visLineReducer, Color.blue);
			if (tire.steerCoefficient == 0)
				Debug.DrawLine(tire.transform.position, (Vector2)tire.transform.position + -(Vector2)transform.up * tire.angularVel, Color.green);
		}
		foreach (var forcePosition in forcePositions)
		{
			rb.AddForceAtPosition(forcePosition.force / 4, forcePosition.pos, ForceMode2D.Force);
		}
	}
	private void HandleAudio()
	{
		float tireAngularVelSum = 0;
		int acceleratingTiresCount = 0;
		foreach (var tire in tires)
		{
			if (tire.accelerationCoefficient == 0)
				continue;
			tireAngularVelSum += math.abs(tire.angularVel);
			acceleratingTiresCount++;
		}
		if (acceleratingTiresCount > 0)
		{
			float rpm = tireAngularVelSum / acceleratingTiresCount;
			carAudioController.rpm = rpm / 700;
		}
	}
	private void SetCenterOfMassObj()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).name != "CenterOfMass")
				continue;
			centerOfMass = transform.GetChild(i);
			break;
		}
	}
}
