using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public Text errorMessage;

    public void SetText(string text) {
        errorMessage.text = text;
    }

    public void CloseBox() {
        Destroy(gameObject);
    }

}
