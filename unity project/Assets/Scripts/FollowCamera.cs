using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public PlayerMovement car;
    public Vector3 cameraOffset;
    public Camera cam;
    float speed = 7.5f;
    void LateUpdate() {
        if (!car) { return; }
        transform.position = Vector3.Lerp(transform.position, car.transform.position + cameraOffset + (Vector3)car.ForwardVelocity() * .333f, speed * Time.deltaTime);
        float t = Mathf.InverseLerp(0, 10, Mathf.Abs(car.rb.velocity.magnitude));
        cam.orthographicSize = Mathf.Lerp(5, 8, t);
    }
}
