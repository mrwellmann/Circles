using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Circles
{
    public class DeviceShakeDetection : MonoBehaviour
    {
        public event Action OnDeviceShakeDetected;

        private float accelerometerUpdateInterval = 1.0f / 60.0f;

        // The greater the value of LowPassKernelWidthInSeconds, the slower the
        // filtered value will converge towards current input sample (and vice versa).
        private float _lowPassKernelWidthInSeconds = 1.0f;

        // This next parameter is initialized to 2.0 per Apple's recommendation,
        // or at least according to Brady! ;)
        private float shakeDetectionThreshold = 1.5f;

        private float lowPassFilterFactor;
        private Vector3 lowPassValue;

        private void Start()
        {
            SetupDeviceShake();
        }

        private void Update()
        {
            RecognizeDeviceShake();
        }

        private void SetupDeviceShake()
        {
            lowPassFilterFactor = accelerometerUpdateInterval / _lowPassKernelWidthInSeconds;
            shakeDetectionThreshold *= shakeDetectionThreshold;
            lowPassValue = Input.acceleration;
        }

        private void RecognizeDeviceShake()
        {
            Vector3 acceleration = Input.acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
            Vector3 deltaAcceleration = acceleration - lowPassValue;

            if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
            {
                // Perform your "shaking actions" here. If necessary, add suitable
                // guards in the if check above to avoid redundant handling during
                // the same shake (e.g. a minimum refractory period).
                Debug.Log("Shake event detected at time " + Time.time);
                OnDeviceShakeDetected.Invoke();
            }
        }
    }
}