using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TransformationScaleRotate
{
	public float scale;
	public float rotate;

	public Matrix4x4 GetTransformation()
	{
		var rotateMatrix = new Matrix4x4();
		rotateMatrix.m00 = Mathf.Cos(rotate); rotateMatrix.m01 = -Mathf.Sin(rotate); rotateMatrix.m02 = 0; rotateMatrix.m03 = 0;
		rotateMatrix.m10 = Mathf.Sin(rotate); rotateMatrix.m11 = Mathf.Cos(rotate); rotateMatrix.m12 = 0; rotateMatrix.m13 = 0;
		rotateMatrix.m20 = 0; rotateMatrix.m21 = 0; rotateMatrix.m22 = 1; rotateMatrix.m23 = 0;
		rotateMatrix.m30 = 0; rotateMatrix.m31 = 0; rotateMatrix.m32 = 0; rotateMatrix.m33 = 1;

		var scaleMatrix = new Matrix4x4();
		scaleMatrix.m00 = scale; scaleMatrix.m01 = 0; scaleMatrix.m02 = 0; scaleMatrix.m03 = 0;
		scaleMatrix.m10 = 0; scaleMatrix.m11 = scale; scaleMatrix.m12 = 0; scaleMatrix.m13 = 0;
		scaleMatrix.m20 = 0; scaleMatrix.m21 = 0; scaleMatrix.m22 = scale; scaleMatrix.m23 = 0;
		scaleMatrix.m30 = 0; scaleMatrix.m31 = 0; scaleMatrix.m32 = 0; scaleMatrix.m33 = scale;

		return rotateMatrix * scaleMatrix;
	}
}

public class FractalEditor : MonoBehaviour 
{
	public int iterationCount = 10;
	public MetricEditor metricEditor;
	public TransformationScaleRotate[] transformations;
	private Rect bounds;

	private void Start()
	{
		var rectTransform = GetComponent<RectTransform>();
		bounds = GetComponent<RectTransform>().rect;
		bounds.center = rectTransform.anchoredPosition;
		bounds.size = rectTransform.sizeDelta;
	}

	public void Update()
	{
		var currentMetric = metricEditor.metric;

		// Calculate all transformations
		var matrices = new List<Matrix4x4>();
		foreach (var transformation in transformations)
			matrices.Add(transformation.GetTransformation());

		for (int i = 0; i < iterationCount; i++)
		{
			var newMetric = new Metric();

			foreach (var point in currentMetric.points)
			{
				var transformedPoint = new Vector2();
				foreach (var matrix in matrices)
				{
					var tempPoint = new Vector4(point.x, point.y, 0, 1);
					tempPoint = matrix.MultiplyPoint(tempPoint);
					transformedPoint += new Vector2(tempPoint.x, tempPoint.y);
				}

				newMetric.points.Add(transformedPoint);
			}

			currentMetric = newMetric;
		}

		// Draw metric carcas
		var points = currentMetric.points;
		for (int i = 1; i < points.Count; i++)
			Debug.DrawLine(points[i-1] + bounds.center, points[i] + bounds.center, new Color(0, 255, 0, 255));
	}
}
