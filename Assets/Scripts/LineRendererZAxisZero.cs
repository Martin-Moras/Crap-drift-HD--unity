using UnityEngine;

public class LineRendererZAxisZero : MonoBehaviour
{
	public LineRenderer line;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		line = GetComponent<LineRenderer>();
		for (int i = 0; i < line.positionCount; i++)
		{
			line.SetPosition(i, new Vector3(line.GetPosition(i).x, line.GetPosition(i).y, 0));
		}

	}
}
