using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject basicCirclePrefab;

    [SerializeField]
    private float circleSizeSteps = 0.025f;

    private List<Circle> circles;
    public bool GravityEnabled { get; private set; }

    private void Awake()
    {
        circles = gameObject.GetComponentsInChildren<Circle>().ToList();
        Reset();
    }

    public void Reset()
    {
        for (int i = 0; i < circles.Count(); i++)
        {
            Destroy(circles[i].gameObject);
        }
        circles.Clear();

        var circleObject = GameObject.Instantiate(basicCirclePrefab, gameObject.transform);

        var circle = circleObject.GetComponent<Circle>();
        circles.Add(circle);

        SetGravityState(isActive: false);
    }

    public void SetGravityState(bool isActive)
    {
        GravityEnabled = isActive;
        foreach (var circle in circles)
        {
            circle.GravityEnabled = isActive;
        }
    }

    public void CreateCircleAtPosition(Vector3 position = default, Quaternion rotation = default)
    {
        var circleObject = GameObject.Instantiate(basicCirclePrefab, position, rotation, gameObject.transform);
        var circle = circleObject.GetComponent<Circle>();
        circle.GravityEnabled = GravityEnabled;
        circle.SetRandomVisualization();
        circles.Add(circle);
    }

    internal void IncreaseCircleSize()
    {
        foreach (var circle in circles)
        {
            var targetScale = circle.transform.localScale * circleSizeSteps;
            circle.transform.DOScale(targetScale, 0.25f);
        }
    }

    internal void IncreaseCircleSize(float sizeChange)
    {
        foreach (var circle in circles)
        {
            var targetScale = circle.transform.localScale * sizeChange;
            circle.transform.DOScale(targetScale, 0.25f);
        }
    }
}