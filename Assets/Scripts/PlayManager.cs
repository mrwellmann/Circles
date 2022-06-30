using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    private DeviceShakeDetection _deviceShakeDetection;
    private CircleManager _ballManager;

    private void Awake()
    {
        _deviceShakeDetection = gameObject.AddComponent<DeviceShakeDetection>();
        _deviceShakeDetection.DeviceShakeDetected += OnShakeDetected;

        _ballManager = gameObject.GetComponentInChildren<CircleManager>();
    }

    private void Update()
    {
        // Set gravity direction depending to the device rotation
        Physics2D.gravity = 9.82f * Input.acceleration.normalized;

        if (Input.GetKeyDown(KeyCode.Escape))
            Restart();
    }

    private void OnShakeDetected()
    {
        _ballManager.SetGravityState(isActive: true);
    }

    private void Restart()
    {
        _ballManager.Reset();
    }
}