using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;

public class GameManager : MonoBehaviour
{
    List<PlayerMovement> players;
    public GameObject carObject;
    public FollowCamera cam;
    public Transform spawnPoint;
    public LeaderboardUI leaderboardUI;
    public Material ghostMat;
    public StartButton startUI;
    public Canvas canvas;
    public GameObject popupbox;
    public GameObject finishPanel;

    void Start() {
        players = new List<PlayerMovement>();
    }

    public IEnumerator Login(string name, string url) {
        string json = "{\"name\":\"" + name + "\"}";
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            Popup popupBox = Instantiate(popupbox).GetComponent<Popup>();
            popupBox.transform.SetParent(canvas.transform, false);
            popupBox.SetText("Cannot connect to the game server. Please try again later.");
        }
        else
        {
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(uwr.downloadHandler.text);
            if (loginResponse.success) {
                SpawnPlayer(loginResponse.name);
                startUI.startPanel.SetActive(false);
                StartCoroutine(GetLeaderboardData("http://redisracing.com:3000/leaderboard"));
            }
            else {
                Popup popupBox = Instantiate(popupbox).GetComponent<Popup>();
                popupBox.transform.SetParent(canvas.transform, false);
                popupBox.SetText(loginResponse.message);
            }
        }
    }
    public void SpawnPlayer(string name) {
        // Loop through all players to check if the current user already has a car
        foreach (PlayerMovement player in players) {
            if (player.self) {
                return;
            }
        }

        PlayerMovement car = Instantiate(carObject, spawnPoint.position, spawnPoint.transform.rotation).GetComponent<PlayerMovement>();
        car.SetName(name);
        car.self = true;
        players.Add(car);
        cam.car = car;
        car.isRacing = true;
        car.timerCoroutine = StartCoroutine(car.StartTimer());
        StartCoroutine(PostPlayerData("http://redisracing.com:3000/updatepos", car));
    }

    public void RestartPlayer(PlayerMovement car) {
        StopCoroutine(car.timerCoroutine);
        finishPanel.SetActive(false);
        car.rb.velocity = new Vector2(0,0);
        car.transform.position = spawnPoint.transform.position;
        car.transform.rotation = spawnPoint.transform.rotation;
        cam.transform.position = car.transform.position;
        car.isRacing = true;
        car.timerCoroutine = StartCoroutine(car.StartTimer());
    }

    IEnumerator PostPlayerData(string url, PlayerMovement selfCar) {
        while(selfCar) {
            DateTime time = DateTime.Now;
            PlayerData playerData = new PlayerData();
            playerData.name = selfCar.playerName;
            playerData.xPos = selfCar.transform.position.x;
            playerData.yPos = selfCar.transform.position.y;
            playerData.zRot = selfCar.transform.eulerAngles.z;
            string json = JsonUtility.ToJson(playerData);
            var uwr = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                //Debug.LogError("Received: " + uwr.downloadHandler.text);
                var playersJson = JsonConvert.DeserializeObject<List<PlayerData>>(uwr.downloadHandler.text);
                foreach (PlayerData player in playersJson) {
                    bool found = false;
                    bool self = false;
                    foreach (PlayerMovement player2 in players) {
                        if (player.name == player2.playerName) {
                            if (player2.self) {
                                self = true;
                                break;
                            }
                            player2.time = 0;
                            player2.oldPos = player2.transform.position;
                            player2.newPos = new Vector3(player.xPos, player.yPos, 0);
                            player2.oldRot = player2.transform.rotation.eulerAngles.z;
                            player2.newRot = player.zRot;
                            player2.lastPing = DateTime.Parse(player.lastping);
                            found = true;
                            break;
                        }
                    }
                    if (!found && !self) {
                        PlayerMovement car = Instantiate(carObject, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<PlayerMovement>();
                        players.Add(car);
                        car.GetComponent<BoxCollider2D>().isTrigger = true;
                        car.time = 0;
                        car.SetName(player.name);
                        car.oldPos = car.transform.position;
                        car.newPos = new Vector3(player.xPos, player.yPos, 0);
                        car.oldRot = car.transform.rotation.z;
                        car.newRot = player.zRot;
                        car.lastPing = DateTime.Parse(player.lastping);
                        StartCoroutine(car.AFKCheck());

                        // Set material to ghost
                        car.GetComponent<SpriteRenderer>().material = ghostMat;
                    }
                }
            }
            DateTime time2 = DateTime.Now;
            float timePassed = (float)time2.Subtract(time).TotalSeconds;
            if (timePassed < .1f) {
                yield return new WaitForSeconds(.1f - timePassed);
            }
        }
	}
    public IEnumerator GetLeaderboardData(string url) {
        var uwr = new UnityWebRequest(url, "GET");
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else {
            Debug.Log(uwr.downloadHandler.text);
            JsonToLeaderboard(uwr.downloadHandler.text);
        }
    }
    public void JsonToLeaderboard(string json) {
        var leaderboard = JsonConvert.DeserializeObject<List<LeaderboardData>>(json);
        leaderboard = leaderboard.OrderBy(p => p.laptime).ToList();
        leaderboardUI.UpdateLeaderboard(leaderboard);
    }

    public void StartGetLeaderboard() {
        StartCoroutine(GetLeaderboardData("http://redisracing.com:3000/leaderboard"));
    }

    public void Finish() {
        finishPanel.SetActive(true);
        StartCoroutine(finishPanel.GetComponent<FinishUI>().FadeIn());
    }

    public void RemovePlayer(PlayerMovement car) {
        players.Remove(car);
    }
    class PlayerData {
        public string name;
        public float xPos;
        public float yPos;
        public float zRot;
        public string lastping;
    }

    class LoginResponse {
        public string name;
        public bool success;
        public string message;
    }
}
