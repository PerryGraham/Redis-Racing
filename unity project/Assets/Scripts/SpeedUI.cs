using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour
{
    public Text text;
    
    public void SetSpeed(float speed) {
        text.text = Mathf.Round(speed * 10).ToString();
    }
}
