using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metric
{
	public List<Vector2> points;

	public Metric()
	{
		points = new List<Vector2>();
	}

	public void AddPoint(Vector2 point)
	{
		points.Add(point);
	}

	public void Clear()
	{
		points.Clear();
	}
}

public class MetricEditor : MonoBehaviour 
{
	public float gridSize = 0.1f;
	public Metric metric;
	private bool isMetricCycle;
	private Rect bounds;
	private LineRenderer lineRenderer;

	private Vector2 GetSnapedPoint(Vector2 point)
	{
		var positionInGridX = (int)(point.x / gridSize);
		var positionInGridY = (int)(point.y / gridSize);
		return new Vector2(positionInGridX * gridSize, positionInGridY * gridSize);
	}

	private void Start()
	{
		metric = new Metric();

		lineRenderer = GetComponent<LineRenderer>();

		var rectTransform = GetComponent<RectTransform>();
		bounds = GetComponent<RectTransform>().rect;
		bounds.center = rectTransform.anchoredPosition;
		bounds.size = rectTransform.sizeDelta;
	}

	private void Update()
	{
		// Draw metric carcas
		var points = metric.points;
		for (int i = 1; i < points.Count; i++)
			Debug.DrawLine(points[i-1] + bounds.center, points[i] + bounds.center, new Color(0, 255, 0, 255));

		var mousePosition = GetSnapedPoint(Input.mousePosition);

		if (!bounds.Contains(mousePosition))
			return;

		// Check if we want to add new point
		if (Input.GetMouseButtonUp(0))
		{
			if (isMetricCycle)
			{
				metric.Clear();
				isMetricCycle = false;
			}

			metric.AddPoint(mousePosition - bounds.center);

			if (points.Count != 1 && mousePosition == points[0])
				isMetricCycle = true;

			Debug.Log(string.Format("Adding point {0}", mousePosition));
		}

		// Draw preliminary point
		if (points.Count != 0 && !isMetricCycle)
		{
			Debug.DrawLine(points[points.Count - 1] + bounds.center, mousePosition, new Color(0, 255, 0, 128));
		}
	}
}
