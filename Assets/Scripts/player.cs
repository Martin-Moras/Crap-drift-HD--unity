using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	public Rigidbody2D rb { get; private set; }
	private InputAction steeringInput;
	private InputAction accelerationInput;
	private InputAction handBreakInput;
	public List<TireBehaviour> tires;
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
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
		Camera.main.transform.position = transform.position + new Vector3(0, 0, -10);
	}
	private void FixedUpdate()
	{
		ManageTires();

	}
	private void ManageTires()
	{
		foreach (TireBehaviour tire in tires)
		{
			tire.steerInput = steeringInput.ReadValue<float>();
			tire.accelerationInput = accelerationInput.ReadValue<float>();
			tire.isHandBreaking = handBreakInput.IsPressed();
			
			Vector2 frictionForce = tire.GetFrictionForces(rb);
			rb.AddForceAtPosition(frictionForce, tire.relativePosition);
		}
	}
}
