using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Finish : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerMovement player = col.gameObject.GetComponent<PlayerMovement>();
        if (!player.self) { return; }
        player.isRacing = false;
        FinishData finishData = new FinishData();
        finishData.name = player.playerName;
        finishData.time = player.timer;
        StartCoroutine(SendTime("http://localhost:80/finish", finishData));
        player.ResetTimer();
    }
    
    IEnumerator SendTime(string url, FinishData finishData) {
        string json = JsonUtility.ToJson(finishData);
        Debug.Log(json);
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError) {
            Debug.LogError("Error While Sending: " + uwr.error);
        }
        
        else {
            Debug.LogError("Received: " + uwr.downloadHandler.text);
        }
    }
    class FinishData {
        public string name;
        public float time;
    }
}
