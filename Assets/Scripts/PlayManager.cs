using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using UnityEngine.EventSystems;

public class PlayManager : MonoBehaviour
{
    private DeviceShakeDetection _deviceShakeDetection;
    private CircleManager _ballManager;
    private bool _wasZoomingLastFrame;
    private Vector2 _lastPanPosition;
    private int _panFingerId;
    private List<Circle> _lastTouchedCircles;

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

        HandleTouchImput();
    }

    private void HandleTouchImput()
    {
        PointerEventData[] pointer;
        List<RaycastResult>[] raycastResult;

        switch (Input.touchCount)
        {
            case 1: // One Finger
                _wasZoomingLastFrame = false;
                OneFingerInteractions(out pointer, out raycastResult);
                break;

            case 2: // Two Finger
                TwoFingerInteractions(out pointer, out raycastResult);
                break;

            default:
                _wasZoomingLastFrame = false;
                break;
        }

        //PointerEventData pointer = new PointerEventData(EventSystem.current);
        //List<RaycastResult> raycastResult = new List<RaycastResult>();

        //for (int i = 0; i < Input.touchCount; ++i)
        //{
        //    Debug.Log($"TapCount for {i} is {Input.GetTouch(i).tapCount}");

        //    if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
        //    {
        //        pointer.position = Input.GetTouch(i).position;

        //        //var objectHit = EventRay(pointer, raycastResult);
        //        var objectHit = TryGetCameraRayHit(pointer, raycastResult);

        //        if (!objectHit)
        //        {
        //            // Construct a ray from the current touch coordinates
        //            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
        //            _ballManager.CreateCircleAtPosition(position: new Vector3(ray.origin.x, ray.origin.y, 0));
        //        }
        //    }
        //}
    }

    private void OneFingerInteractions(out PointerEventData[] pointer, out List<RaycastResult>[] raycastResult)
    {
        pointer = new PointerEventData[] { new PointerEventData(EventSystem.current) };
        raycastResult = new List<RaycastResult>[] { new List<RaycastResult>() };

        Touch touch = Input.GetTouch(0);
        pointer[0].position = touch.position;

        if (Input.GetTouch(0).phase == TouchPhase.Began) //Touch started this frame
        {
            _lastPanPosition = touch.position;
            _panFingerId = touch.fingerId;

            if (TryGetCameraRayHit(pointer[0], ref raycastResult[0]))
            {
                if (TryGetCirclesHit(raycastResult[0], out _lastTouchedCircles))
                {
                    foreach (Circle circle in _lastTouchedCircles)
                        circle.OnTap();
                }
            }
            else
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(pointer[0].position);
                _ballManager.CreateCircleAtPosition(position: new Vector3(position.x, position.y, 0));
            }
        }
        else if (touch.fingerId == _panFingerId && touch.phase == TouchPhase.Moved)
        {
            if (_lastTouchedCircles.Count > 0) //drag circle
            {
                Vector3 lastPos = Camera.main.ScreenToWorldPoint(_lastPanPosition);
                Vector3 newPos = Camera.main.ScreenToWorldPoint(touch.position);
                Vector3 difPos = newPos - lastPos;
                Vector3 move = new Vector3(difPos.x, difPos.y, 0);
                _lastPanPosition = touch.position;

                foreach (Circle circle in _lastTouchedCircles)
                {
                    circle.GravityEnabled = false;
                    circle.transform.Translate(move, Space.World);
                }
            }
        }
        else if (_lastTouchedCircles.Count > 0 && touch.phase == TouchPhase.Ended)
        {
            foreach (Circle circle in _lastTouchedCircles) //enable gravity if it was deactivated
            {
                circle.GravityEnabled = _ballManager.GravityEnabled;
            }
            _lastTouchedCircles.Clear();
        }
    }

    private static void TwoFingerInteractions(out PointerEventData[] pointer, out List<RaycastResult>[] raycastResult)
    {
        pointer = new PointerEventData[] { new PointerEventData(EventSystem.current), new PointerEventData(EventSystem.current) };
        raycastResult = new List<RaycastResult>[] { new List<RaycastResult>(), new List<RaycastResult>() };

        Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };

        pointer[0].position = Input.GetTouch(0).position;
        pointer[1].position = Input.GetTouch(1).position;

        //if (!_wasZoomingLastFrame)
        //{
        //    lastZoomPositions = newPositions;
        //    _wasZoomingLastFrame = true;
        //}
        //else
        //{
        //    // Zoom based on the distance between the new positions compared to the
        //    // distance between the previous positions.
        //    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
        //    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
        //    float position = newDistance - oldDistance;

        //    ZoomCamera(position, ZoomSpeedTouch);

        //    lastZoomPositions = newPositions;
        //}
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

    private bool TryGetCameraRayHit(PointerEventData pointer, ref List<RaycastResult> raycastResult)
    {
        var caster = Camera.main.gameObject.GetComponent<Physics2DRaycaster>();
        caster.Raycast(pointer, raycastResult);
        return raycastResult.Count > 0 ? true : false;
    }

    private bool TryGetCirclesHit(List<RaycastResult> raycastResult, out List<Circle> circles)
    {
        circles = new List<Circle>();
        foreach (RaycastResult result in raycastResult)
        {
            Circle circle = result.gameObject.GetComponent<Circle>();
            if (circle != null)
            {
                circles.Add(circle);
            }
        }

        return circles.Count > 0 ? true : false;
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void OnShakeDetected()
    {
        _ballManager.SetGravityState(isActive: true);
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void Restart()
    {
        _ballManager.Reset();
    }
}