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
    public void SpawnPlayer(string name) {
        PlayerMovement car = Instantiate(carObject, spawnPoint.position, spawnPoint.transform.rotation).GetComponent<PlayerMovement>();
        car.SetName(name);
        car.self = true;
        players = new List<PlayerMovement>();
        players.Add(car);
        cam.car = car.transform;
        car.isRacing = true;
        car.timerCoroutine = StartCoroutine(car.StartTimer());
        StartCoroutine(PostPlayerData("http://localhost:80/updatepos", car));
    }

    public void RestartPlayer(PlayerMovement car) {
        StopCoroutine(car.timerCoroutine);
        car.moveSpeed = 0;
        car.transform.position = spawnPoint.transform.position;
        car.transform.rotation = spawnPoint.transform.rotation;
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
                        car.time = 0;
                        car.SetName(player.name);
                        car.oldPos = car.transform.position;
                        car.newPos = new Vector3(player.xPos, player.yPos, 0);
                        car.oldRot = car.transform.rotation.z;
                        car.newRot = player.zRot;
                        car.lastPing = DateTime.Parse(player.lastping);
                        StartCoroutine(car.AFKCheck());
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
        Debug.Log(json);
        var leaderboard = JsonConvert.DeserializeObject<List<LeaderboardData>>(json);
        leaderboard = leaderboard.OrderBy(p => p.laptime).ToList();
        leaderboardUI.UpdateLeaderboard(leaderboard);
    }
    class PlayerData {
        public string name;
        public float xPos;
        public float yPos;
        public float zRot;
        public string lastping;
    }
}
