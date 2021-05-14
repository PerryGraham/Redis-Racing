using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    float accSpeed = 2.5f;
    float rotSpeed = 100f;
    float driftAmount = .1f;
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
    float turnTime = 0f;
    public bool isRacing;
    public NameUI nameUI;
    public SpeedUI speedUI;
    public TimerUI timerUI;
    public Coroutine timerCoroutine;
    public GameObject screenSpaceCanvas;
    Tilemap track;
    void Start() {
        if (self) {
            screenSpaceCanvas.SetActive(true);
        }
        GameObject grid = GameObject.Find("Grid");
        track = grid.transform.Find("Track").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (self) {
            rotation = Input.GetAxisRaw("Horizontal");
            movement = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Horizontal")) {
                turnTime = Time.time;
            }
            if (Input.GetButtonUp("Horizontal")) {
                turnTime = 0;
            }

            if (Input.GetButtonDown("Restart")) {
                Restart();
            }

            return;
        }
    }

    void FixedUpdate() {
        if (self) {
            if (rb.velocity.magnitude < 15f) {
                rb.AddForce(transform.up * accSpeed * movement);
            }

            rb.velocity = ForwardVelocity() + RightVelocity() * driftAmount;

            float angularVel = Mathf.Lerp(0, rotSpeed, rb.velocity.magnitude / 3);
            rb.angularVelocity = rotation * -angularVel;

            Vector3Int trackTilePos = track.WorldToCell(transform.position);
            Tile trackTile = track.GetTile<Tile>(trackTilePos);
            if (trackTile) {
                rb.drag = Mathf.Clamp(rb.drag - .01f, .1f, 1f);
                driftAmount = .1f;
            }
            else {
                rb.drag = 1f;
                driftAmount = .95f;
            }

            // Update speed UI
            speedUI.SetSpeed(rb.velocity.magnitude);
        }
        else {
            time += Time.fixedDeltaTime;
            rb.MovePosition(Vector3.Lerp(oldPos, newPos, time / .1f));
            rb.MoveRotation(Quaternion.Lerp(Quaternion.Euler(0, 0, oldRot), Quaternion.Euler(0, 0, newRot), time / .1f));
        }
    }

    public IEnumerator AFKCheck() {
        bool isAFK = false;
        while(!isAFK) {
            if (lastPing.AddSeconds(30) < DateTime.Now) {
                isAFK = true;
                GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                gameManager.StartGetLeaderboard();
                gameManager.RemovePlayer(this);
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
        timerUI.ResetPosition();
    }
    public void SetName(string name) {
        playerName = name;
        nameUI.SetName(name);
    }
    public void Restart() {
        ResetTimer();
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.RestartPlayer(this);
    }
    public Vector2 ForwardVelocity() {
        return transform.up * Vector2.Dot(rb.velocity, transform.up);
    }
    public Vector2 RightVelocity() {
        return transform.right * Vector2.Dot(rb.velocity, transform.right);
    }
}