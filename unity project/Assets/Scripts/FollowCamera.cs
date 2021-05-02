using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform car;
    public Vector3 cameraOffset;
    float smoothSpeed = 7.5f;
    void LateUpdate() {
        if (!car) { return; }
        transform.position = Vector3.Lerp(transform.position, car.transform.position + cameraOffset, smoothSpeed * Time.deltaTime);
    }
}
