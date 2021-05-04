using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform car;
    public Vector3 cameraOffset;
    float positionSpeed = 7.5f;
    float rotationSpeed = 7.5f;
    void LateUpdate() {
        if (!car) { return; }
        transform.position = Vector3.Lerp(transform.position, car.transform.position + cameraOffset, positionSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, car.rotation, rotationSpeed * Time.deltaTime);
    }
}
