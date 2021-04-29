using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    float moveSpeed = 0f;
    float accSpeed = .1f;
    float rotSpeed = 100f;
    float minSpeed = -5f;
    float maxSpeed = 10f;
    float movement;
    public float rotation;
    public string playerName = "temp";
    public Rigidbody2D rb;
    public bool self = false;
    public Vector3 oldPos;
    public Vector3 newPos;
    public float oldRot;
    public float newRot;
    public float time = 0f;
    public DateTime lastPing;

    void Start() {

    }

    // Update is called once per frame
    void Update()
    {
        if (self) {
            rotation = Input.GetAxisRaw("Horizontal");
            movement = Input.GetAxisRaw("Vertical");
            return;
        }
        rb.MovePosition(Vector3.Lerp(oldPos, newPos, time / .1f));
        rb.MoveRotation(Quaternion.Lerp(Quaternion.Euler(0, 0, oldRot), Quaternion.Euler(0, 0, newRot), time / .1f));
        time += Time.deltaTime;
    }

    void FixedUpdate() {
        if (self) {
            moveSpeed = Mathf.Clamp(accSpeed * movement + moveSpeed, minSpeed, maxSpeed);
            if (movement == 0) {
                moveSpeed -= (moveSpeed >= 0 ? accSpeed : -accSpeed);
                moveSpeed = (moveSpeed < 0.5f && moveSpeed > -0.5f ? 0 : moveSpeed);
            }
            rb.MovePosition(rb.position + new Vector2(transform.up.x, transform.up.y) * moveSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation + rotSpeed * -rotation * Time.fixedDeltaTime);
        }
    }

    public IEnumerator AFKCheck() {
        bool isAFK = false;
        while(!isAFK) {
            if (lastPing.AddSeconds(30) < DateTime.Now) {
                isAFK = true;
                Debug.Log("Destroy");
                Destroy(gameObject);

            }
            else {
                Debug.Log("No");
                yield return new WaitForSeconds(30);
            }
        }
    }
}