using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 0f;
    float accSpeed = .05f;
    float rotSpeed = 100f;
    float minSpeed = -5f;
    float maxSpeed = 15f;
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
    public float timer = 0f;
    public bool isRacing;
    public NameUI nameUI;
    public SpeedUI speedUI;
    public TimerUI timerUI;
    public Coroutine timerCoroutine;
    GameManager gameManager;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (self) {
            rotation = Input.GetAxisRaw("Horizontal");
            movement = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Restart")) {
                Restart();
            }
            return;
        }
    }

    void FixedUpdate() {
        if (self) {
            moveSpeed = Mathf.Clamp(accSpeed * movement + moveSpeed, minSpeed, maxSpeed);
            if (movement == 0) {
                moveSpeed -= (moveSpeed >= 0 ? accSpeed : -accSpeed);
                moveSpeed = (moveSpeed < 0.5f && moveSpeed > -0.5f ? 0 : moveSpeed);
            }
            // Only allow rotation if the player is moving
            if (moveSpeed > 0 || moveSpeed < 0) {
                rb.MoveRotation(rb.rotation + rotSpeed * -rotation * Time.fixedDeltaTime);
            }
            rb.MovePosition(rb.position + new Vector2(transform.up.x, transform.up.y) * moveSpeed * Time.fixedDeltaTime);

            // Update speed UI
            speedUI.SetSpeed(moveSpeed);
        }
        else {
            time += Time.fixedDeltaTime;
            rb.MovePosition(Vector3.Lerp(oldPos, newPos, time / .1f));
            rb.MoveRotation(Quaternion.Lerp(Quaternion.Euler(0, 0, oldRot), Quaternion.Euler(0, 0, newRot), time / .1f));
        }
    }

    void OnCollisionStay2D(Collision2D col) {
        moveSpeed = moveSpeed / 1.05f;
    }

    public IEnumerator AFKCheck() {
        bool isAFK = false;
        while(!isAFK) {
            if (lastPing.AddSeconds(30) < DateTime.Now) {
                isAFK = true;
                Destroy(gameObject);

            }
            else {
                yield return new WaitForSeconds(30);
            }
        }
    }
    
    public IEnumerator StartTimer() {
        while(isRacing) {
            timer += Time.deltaTime;
            timerUI.SetTime(timer);
            yield return 0;
        }
    }

    public void ResetTimer() {
        timer = 0f;
        }

    public void SetName(string name) {
        playerName = name;
        nameUI.SetName(name);
        }
    public void Restart() {
        ResetTimer();
        gameManager.RestartPlayer(this);
    }
}