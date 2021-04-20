using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{

    PlayerMovement[] players;

    // Start is called before the first frame update
    void Start()
    {
        PlayerData playerData = new PlayerData();
        playerData.xPos = transform.position.x;
        playerData.yPos = transform.position.y;
        playerData.zRot = transform.rotation.z;
        string json = JsonUtility.ToJson(playerData);
        StartCoroutine(PostPlayerData("http://localhost:80/updatepos", json));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

IEnumerator PostPlayerData(string url, string json)
	{
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
    	    Debug.Log("Received: " + uwr.downloadHandler.text);
            // Players playersJson = JsonUtility.FromJson<Players>(uwr.downloadHandler.text);
            // foreach (PlayerMovement player in players) {
            //     foreach (PlayerData player2 in playersJson.players) {
            //         if (player.name == player2.name) {
            //             player.UpdatePosRot(new Vector3 (player2.xPos, player2.yPos, 0), player2.zRot);
            //         }
            //     }
            // }

    	}
	}

    private class PlayerData {
        public float xPos;
        public float yPos;
        public float zRot;
        public string name;
    }
    private class Players {
        public PlayerData[] players;
    }
}
