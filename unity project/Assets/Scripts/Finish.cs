using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Finish : MonoBehaviour
{
    GameManager gameManager;
    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerMovement player = col.gameObject.GetComponent<PlayerMovement>();
        if (player.self) {
            if (player.isRacing) {
                player.isRacing = false;
                FinishData finishData = new FinishData();
                finishData.name = player.playerName;
                finishData.time = player.timer;
                StartCoroutine(SendTime("http://localhost:3000/finish", finishData));
                player.timerUI.FinishTween();
                gameManager.Finish();
            }
        }
        else {
            StartCoroutine(gameManager.GetLeaderboardData("http://localhost:3000/leaderboard"));
        }
    }
    
    IEnumerator SendTime(string url, FinishData finishData) {
        string json = JsonUtility.ToJson(finishData);
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        
        else {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            gameManager.JsonToLeaderboard(uwr.downloadHandler.text);
        }
    }
    class FinishData {
        public string name;
        public float time;
    }
}
