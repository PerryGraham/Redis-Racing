using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    List<PlayerMovement> players;
    public GameObject carObject;
    public FollowCamera cam;
    public void SpawnPlayer(string name) {
        PlayerMovement car = Instantiate(carObject, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<PlayerMovement>();
        car.playerName = name;
        car.self = true;
        players = new List<PlayerMovement>();
        players.Add(car);
        cam.car = car.transform;
        StartCoroutine(PostPlayerData("http://localhost:80/updatepos", car));
    }

IEnumerator PostPlayerData(string url, PlayerMovement selfCar)
	{
        while(true) {
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
                Debug.LogError("Error While Sending: " + uwr.error);
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
                        car.playerName = player.name;
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
            Debug.Log(DateTime.Now.Subtract(time).TotalSeconds);
        }
	}
    [Serializable]
    private class PlayerData {
        public string name;
        public float xPos;
        public float yPos;
        public float zRot;
        public string lastping;
    }
}
