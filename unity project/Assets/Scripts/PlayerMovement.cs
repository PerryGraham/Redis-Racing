using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float moveSpeed = 0f;
    float accSpeed = .1f;
    float rotSpeed = 100f;
    float minSpeed = -5f;
    float maxSpeed = 10f;

    public Rigidbody2D rb;
    float movement;
    float rotation;

    // Update is called once per frame
    void Update()
    {
        rotation = Input.GetAxisRaw("Horizontal");
        movement = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() {
        moveSpeed = Mathf.Clamp(accSpeed * movement + moveSpeed, minSpeed, maxSpeed);
        if (movement == 0) {
            moveSpeed -= (moveSpeed >= 0 ? accSpeed : -accSpeed);
            moveSpeed = (moveSpeed < 0.5f && moveSpeed > -0.5f ? 0 : moveSpeed);
        }
        Debug.Log(moveSpeed);
        rb.MovePosition(rb.position + new Vector2(transform.up.x, transform.up.y) * moveSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation + rotSpeed * -rotation * Time.fixedDeltaTime);
    }
}