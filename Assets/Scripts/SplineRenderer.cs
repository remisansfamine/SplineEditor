using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(SplineController))]
public class SplineRenderer : MonoBehaviour
{
    private LineRenderer lineRdr = null;
    private SplineController controller = null;

    [SerializeField] private float precision = 0.01f;
    private float step = 0f;

    private void Awake()
    {
        lineRdr = GetComponent<LineRenderer>();
        controller = GetComponent<SplineController>();
    }

    private void OnEnable()
    {
        controller.OnSplineUpdated.AddListener(UpdateLineRenderer);
        UpdateLineRenderer();
    }

    private void OnDisable()
    {
        controller.OnSplineUpdated.RemoveListener(UpdateLineRenderer);
    }

    void UpdateLineRenderer()
    {
        step = 1f / precision;
        lineRdr.positionCount = Mathf.RoundToInt(step) + 1;

        int pointID = 0;
        for (float quantity = 0f; quantity <= 1f; quantity += precision)
        {
            Vector3 LinePoint = controller.EvaluateFromMatrix(quantity);

            lineRdr.SetPosition(pointID, LinePoint);
            pointID++;
        }
    }
}
