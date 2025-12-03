using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
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
	private InputAction handBreakInput;
	public List<TireBehaviour> tires;
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		SetCenterOfMassObj();
		rb.centerOfMass = centerOfMass.localPosition;

		steeringInput = InputSystem.actions.FindAction("Steer");
		accelerationInput = InputSystem.actions.FindAction("Acceleration");
		handBreakInput = InputSystem.actions.FindAction("Handbreak");

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

	}
	private void ManageTires()
	{
		ForcePosition[] forcePositions = new ForcePosition[tires.Count];
		for (int i = 0; i < tires.Count; i++)
		{
			TireBehaviour tire = tires[i];

			tire.steerInput = -steeringInput.ReadValue<float>();
			tire.accelerationInput = accelerationInput.ReadValue<float>();
			tire.isHandBreaking = handBreakInput.IsPressed();

			Vector2 frictionForce = tire.GetFrictionForces(rb);
			forcePositions[i] = new ForcePosition(frictionForce * rb.mass, tire.transform.position);
			// Debug.DrawLine(tire.transform.position, (Vector2)tire.transform.position + frictionForce, Color.blue);
		}
		foreach (var forcePosition in forcePositions)
		{
			rb.AddForceAtPosition(forcePosition.force, forcePosition.pos, ForceMode2D.Force);
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
