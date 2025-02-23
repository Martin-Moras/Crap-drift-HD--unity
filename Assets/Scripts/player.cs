using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
	[SerializeField] Camera mainCam;
	void Update()
	{
		mainCam.transform.position = transform.position + new Vector3(0, 0, -10);
	}
}
