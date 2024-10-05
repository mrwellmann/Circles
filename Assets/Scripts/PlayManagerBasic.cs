using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using UnityEngine.EventSystems;

public class PlayManagerBasic : MonoBehaviour
{
    private DeviceShakeDetection deviceShakeDetection;
    private CircleManager ballManager;

    private Vector2 lastPanPosition;
    float lastDistance = 0;
    private int panFingerId;
    private List<Circle> lastTouchedCircles;

    private void Awake()
    {
        deviceShakeDetection = gameObject.GetComponent<DeviceShakeDetection>();
        ballManager = gameObject.GetComponentInChildren<CircleManager>();
        lastTouchedCircles = new List<Circle>();

        if (deviceShakeDetection != null)
        {
            deviceShakeDetection.OnDeviceShakeDetected += OnShakeDetected;
        }
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
                lastDistance = 0;
                OneFingerInteractions(out pointer, out raycastResult);
                break;

            // case 2: // Two Finger
            //     TwoFingerInteractions(out pointer, out raycastResult);
            //     break;

            default:
                break;
        }
    }

    private void OneFingerInteractions(out PointerEventData[] pointer, out List<RaycastResult>[] raycastResult)
    {
        pointer = new PointerEventData[] { new PointerEventData(EventSystem.current) };
        raycastResult = new List<RaycastResult>[] { new List<RaycastResult>() };

        Touch touch = Input.GetTouch(0);
        pointer[0].position = touch.position;

        if (Input.GetTouch(0).phase == TouchPhase.Began) //Touch started this frame
        {
            lastPanPosition = touch.position;
            panFingerId = touch.fingerId;

            if (TryGetCameraRayHit(pointer[0], ref raycastResult[0]))
            {
                if (TryGetCirclesHit(raycastResult[0], out lastTouchedCircles))
                {
                    foreach (Circle circle in lastTouchedCircles)
                        circle.OnTap();
                }
            }
            else
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(pointer[0].position);
                ballManager.CreateCircleAtPosition(position: new Vector3(position.x, position.y, 0));
            }
        }
        else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
        {
            if (lastTouchedCircles.Count > 0) //drag circle
            {
                Vector3 lastPos = Camera.main.ScreenToWorldPoint(lastPanPosition);
                Vector3 newPos = Camera.main.ScreenToWorldPoint(touch.position);
                Vector3 difPos = newPos - lastPos;
                Vector3 move = new Vector3(difPos.x, difPos.y, 0);
                lastPanPosition = touch.position;

                foreach (Circle circle in lastTouchedCircles)
                {
                    circle.GravityEnabled = false;
                    circle.transform.Translate(move, Space.World);
                }
            }
        }
        else if (lastTouchedCircles?.Count > 0 && touch.phase == TouchPhase.Ended)
        {
            foreach (Circle circle in lastTouchedCircles) //enable gravity if it was deactivated
            {
                circle.GravityEnabled = ballManager.GravityEnabled;
            }
            lastTouchedCircles.Clear();
        }
    }

    // private static void TwoFingerInteractions(out PointerEventData[] pointer, out List<RaycastResult>[] raycastResult)
    // {
    //     pointer = new PointerEventData[] { new PointerEventData(EventSystem.current), new PointerEventData(EventSystem.current) };
    //     raycastResult = new List<RaycastResult>[] { new List<RaycastResult>(), new List<RaycastResult>() };

    //     Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };

    //     pointer[0].position = Input.GetTouch(0).position;
    //     pointer[1].position = Input.GetTouch(1).position;
    // }

    private void TwoFingerInteractions(out PointerEventData[] pointer, out List<RaycastResult>[] raycastResult)
    {
        pointer = new PointerEventData[] { new PointerEventData(EventSystem.current), new PointerEventData(EventSystem.current) };
        raycastResult = new List<RaycastResult>[] { new List<RaycastResult>(), new List<RaycastResult>() };
        pointer[0].position = Input.GetTouch(0).position;
        pointer[1].position = Input.GetTouch(1).position;

        if (Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(1).phase == TouchPhase.Began)
        {
            lastDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
        }
        else if (lastDistance > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
        {
            float currentDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

            float sizeChange = currentDistance / lastDistance;

            sizeChange = sizeChange > 1 ? sizeChange * 1.01f : sizeChange / 1.01f;
            //sizeChange = Mathf.Clamp(sizeChange, 0.9f, 1.1f);

            OnChangeSizeDetected(sizeChange);
            lastDistance = currentDistance;

        }
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
        ballManager.SetGravityState(isActive: true);
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void OnChangeSizeDetected(float sizeChange)
    {
        ballManager.IncreaseCircleSize(sizeChange);
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    private void Restart()
    {
        ballManager.Reset();
    }
}