using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameUI : MonoBehaviour
{
    public Text text;
    public void SetName(string name) {
        text.text = name;
    }
}
