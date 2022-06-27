using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _CirclePrefab;

    private List<Circle> _Circles = new List<Circle>();
    private DeviceShakeDetection _shakeDetection;

    // Start is called before the first frame update
    private void Awake()
    {
        _Circles.AddRange(GetComponentsInChildren<Circle>());

        _shakeDetection = GetComponent<DeviceShakeDetection>();
        _shakeDetection.DeviceShakeDetected += OnDeviceShake;
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
        CreateCircle(CircleType.Red, presSpawn: true);
        CreateCircle(CircleType.Yellow, presSpawn: true);
        CreateCircle(CircleType.Blue, presSpawn: true);
    }

    private void CleanArea(List<Circle> circles)
    {
        foreach (var circle in circles)
        {
            Destroy(circle.gameObject);
        }
        circles.Clear();
    }

    private void CreateCircle(CircleType circleType, int siblingindex = -1, bool presSpawn = false)
    {
        var circleObject = Instantiate(_CirclePrefab);
        circleObject.transform.SetParent(transform);

        if (siblingindex > 0)
            circleObject.transform.SetSiblingIndex(siblingindex);

        var circle = circleObject.GetComponent<Circle>();
        circle.Create(circleType, presSpawn);
        circle.CircleClicked += OnCircleClicked;

        _Circles.Add(circle);
    }

    private void OnCircleClicked(Circle sender)
    {
        CreateCircle(sender.CircleType, sender.transform.GetSiblingIndex() + 1);
    }
}