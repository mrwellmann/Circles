using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _basicCirclePrefab;

    private List<Circle> _circles;

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

        var circleObject = GameObject.Instantiate(_basicCirclePrefab);
        circleObject.transform.SetParent(gameObject.transform, false);

        var circle = circleObject.GetComponent<Circle>();
        _circles.Add(circle);

        SetGravityState(isActive: false);
    }

    public void SetGravityState(bool isActive)
    {
        foreach (var circle in _circles)
        {
            circle.GravityEnabled = isActive;
        }
    }
}