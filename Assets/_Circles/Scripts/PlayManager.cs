using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using UnityEngine.EventSystems;

namespace Circles
{
    public class PlayManager : MonoBehaviour
    {
        private DeviceShakeDetection deviceShakeDetection;
        private CircleManager ballManager;
        private ClapDetector clapDetector;

        private bool wasZoomingLastFrame;
        private Vector2 lastPanPosition;
        private int _panFingerId;
        private List<Circle> lastTouchedCircles;

        private void Awake()
        {
            deviceShakeDetection = gameObject.GetComponent<DeviceShakeDetection>();
            clapDetector = gameObject.GetComponent<ClapDetector>();
            ballManager = gameObject.GetComponentInChildren<CircleManager>();

            if (deviceShakeDetection != null)
            {
                deviceShakeDetection.OnDeviceShakeDetected += OnShakeDetected;
            }

            if (clapDetector != null)
            {
                clapDetector.OnClapDetected += OnClapDetected;
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
                    wasZoomingLastFrame = false;
                    OneFingerInteractions(out pointer, out raycastResult);
                    break;

                case 2: // Two Finger
                    TwoFingerInteractions(out pointer, out raycastResult);
                    break;

                default:
                    wasZoomingLastFrame = false;
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
                lastPanPosition = touch.position;
                _panFingerId = touch.fingerId;

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
            else if (touch.fingerId == _panFingerId && touch.phase == TouchPhase.Moved)
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
            ballManager.SetGravityState(isActive: true);
        }

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        private void OnClapDetected()
        {
            ballManager.IncreaseCircleSize();
        }

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        private void Restart()
        {
            ballManager.Reset();
        }
    }
}