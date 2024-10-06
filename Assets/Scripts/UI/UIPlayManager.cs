using System;
using System.Collections.Generic;
using UnityEngine;

namespace Circles.UI
{
    public class UIPlayManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _CirclePrefab;

        private List<UICircle> _Circles = new List<UICircle>();
        private DeviceShakeDetection _shakeDetection;

        // Start is called before the first frame update
        private void Awake()
        {
            _Circles.AddRange(GetComponentsInChildren<UICircle>());

            _shakeDetection = GetComponent<DeviceShakeDetection>();
            _shakeDetection.OnDeviceShakeDetected += OnDeviceShake;
        }

        private void OnDeviceShake()
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            //Restart();

            //gameObject.GetComponent<BoxCollider2D>().size = new Vector2(
            //    gameObject.GetComponent<RectTransform>().sizeDelta.x,
            //    gameObject.GetComponent<RectTransform>().sizeDelta.y
            //    );
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Restart();
        }

        private void Restart()
        {
            CleanArea(_Circles);
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

        private void CreateCircle(UICircleType circleType, int siblingindex = -1, bool presSpawn = false)
        {
            var circleObject = Instantiate(_CirclePrefab);
            circleObject.transform.SetParent(transform);

            if (siblingindex > 0)
                circleObject.transform.SetSiblingIndex(siblingindex);

            var circle = circleObject.GetComponent<UICircle>();
            circle.Create(circleType, presSpawn);
            circle.CircleClicked += OnCircleClicked;

            _Circles.Add(circle);
        }

        private void OnCircleClicked(UICircle sender)
        {
            CreateCircle(sender.CircleType, sender.transform.GetSiblingIndex() + 1);
        }
    }
}