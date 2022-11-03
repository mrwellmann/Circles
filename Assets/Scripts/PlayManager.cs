using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using UnityEngine.EventSystems;

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

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResult = new List<RaycastResult>();

        for (int i = 0; i < Input.touchCount; ++i)
        {
            Debug.Log($"TapCount for {i} is {Input.GetTouch(i).tapCount}");

            if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
            {
                pointer.position = Input.GetTouch(i).position;

                //var circleWasTaped = EventRay(pointer, raycastResult);
                var circleWasTaped = CameraRay(pointer, raycastResult);

                if (!circleWasTaped)
                {
                    // Construct a ray from the current touch coordinates
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    _ballManager.CreateCircleAtPosition(position: new Vector3(ray.origin.x, ray.origin.y, 0));
                }
            }
        }
    }

    private static bool EventRay(PointerEventData pointer, List<RaycastResult> raycastResult)
    {
        bool hit = false;
        EventSystem.current.RaycastAll(pointer, raycastResult);

        foreach (RaycastResult result in raycastResult)
        {
            //Debug.Log("EventRay");
            Circle circle = result.gameObject.GetComponent<Circle>();
            if (circle != null)
            {
                circle.OnTap();
                hit = true;
            }
        }
        raycastResult.Clear();

        return hit;
    }

    private static bool CameraRay(PointerEventData pointer, List<RaycastResult> raycastResult)
    {
        bool hit = false;

        var caster = Camera.main.gameObject.GetComponent<Physics2DRaycaster>();
        caster.Raycast(pointer, raycastResult);

        foreach (RaycastResult result in raycastResult)
        {
            //Debug.Log("CameraRay");
            Circle circle = result.gameObject.GetComponent<Circle>();
            if (circle != null)
            {
                circle.OnTap();
                hit = true;
            }
        }
        raycastResult.Clear();

        return hit;
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void OnShakeDetected()
    {
        _ballManager.SetGravityState(isActive: true);
    }

    private void Restart()
    {
        _ballManager.Reset();
    }
}