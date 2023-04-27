using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _basicCirclePrefab;

    [SerializeField]
    private float _circelSizeSteps = 0.025f;

    private List<Circle> _circles;
    public bool GravityEnabled { get; private set; }

    private void Awake()
    {
        _circles = gameObject.GetComponentsInChildren<Circle>().ToList();
        Reset();
    }

    public void Reset()
    {
        for (int i = 0; i < _circles.Count(); i++)
        {
            Destroy(_circles[i].gameObject);
        }
        _circles.Clear();

        var circleObject = GameObject.Instantiate(_basicCirclePrefab, gameObject.transform);
        //circleObject.transform.SetParent(gameObject.transform, false);

        var circle = circleObject.GetComponent<Circle>();
        _circles.Add(circle);

        SetGravityState(isActive: false);
    }

    public void SetGravityState(bool isActive)
    {
        GravityEnabled = isActive;
        foreach (var circle in _circles)
        {
            circle.GravityEnabled = isActive;
        }
    }

    public void CreateCircleAtPosition(Vector3 position = default, Quaternion rotation = default)
    {
        var circleObject = GameObject.Instantiate(_basicCirclePrefab, position, rotation, gameObject.transform);
        var circle = circleObject.GetComponent<Circle>();
        circle.GravityEnabled = GravityEnabled;
        circle.SetRandomVisualisation();
        _circles.Add(circle);
    }

    internal void IncreaseCircelSize()
    {
        foreach (var circle in _circles)
        {
            var targetScale = circle.transform.localScale * _circelSizeSteps;// new Vector3(_circelSizeSteps, _circelSizeSteps, _circelSizeSteps);
            circle.transform.DOScale(targetScale, 0.25f);
        }
    }
}