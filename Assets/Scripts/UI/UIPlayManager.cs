using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Circles.UI
{
    public class UIPlayManager : MonoBehaviour
    {
        [FormerlySerializedAs("_CirclePrefab")]
        [SerializeField]
        private GameObject circlePrefab;

        private List<UICircle> circles = new List<UICircle>();
        private DeviceShakeDetection shakeDetection;

        // Start is called before the first frame update
        private void Awake()
        {
            circles.AddRange(GetComponentsInChildren<UICircle>());

            shakeDetection = GetComponent<DeviceShakeDetection>();
            shakeDetection.OnDeviceShakeDetected += OnDeviceShake;
        }

        private void OnDeviceShake()
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Restart();
        }

        private void Restart()
        {
            CleanArea(circles);
            CreateCircle(UICircleType.Red, presSpawn: true);
            CreateCircle(UICircleType.Yellow, presSpawn: true);
            CreateCircle(UICircleType.Blue, presSpawn: true);
        }

        private void CleanArea(List<UICircle> circles)
        {
            foreach (var circle in circles)
            {
                Destroy(circle.gameObject);
            }
            circles.Clear();
        }

        private void CreateCircle(UICircleType circleType, int siblingIndex = -1, bool presSpawn = false)
        {
            var circleObject = Instantiate(circlePrefab);
            circleObject.transform.SetParent(transform);

            if (siblingIndex > 0)
                circleObject.transform.SetSiblingIndex(siblingIndex);

            var circle = circleObject.GetComponent<UICircle>();
            circle.Create(circleType, presSpawn);
            circle.CircleClicked += OnCircleClicked;

            circles.Add(circle);
        }

        private void OnCircleClicked(UICircle sender)
        {
            CreateCircle(sender.CircleType, sender.transform.GetSiblingIndex() + 1);
        }
    }
}