using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartButton : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject startPanel;
    public TMP_InputField textBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick() {
        gameManager.SpawnPlayer(textBox.text);
        startPanel.SetActive(false);
    }
}
