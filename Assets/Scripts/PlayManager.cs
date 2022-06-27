using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{

    void Update()
    {
        Physics2D.gravity = 9.82f * Input.acceleration.normalized;
    }
}
