using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject leaderboardText;
    public void UpdateLeaderboard(List<LeaderboardData> leaderboardData) {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        int count = 0;
        foreach (LeaderboardData player in leaderboardData) {
            if (count > 10) {
                break;
            }
            GameObject newText = Instantiate(leaderboardText);
            newText.transform.SetParent(this.transform, false);
            int minutes = (int) player.laptime / 60;
            float seconds = player.laptime - 60 * minutes;
            newText.GetComponent<Text>().text = string.Format("{0}. {1} | {2:00}:{3:00.00}", count + 1, player.name, minutes, seconds);
            count++;
        }
    }
}
