using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    public Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ToggleDeathMenu(int Score)
    {
        gameObject.SetActive(true);
        scoreText.text = "TEST SCORE TEXT";
        //scoreText.text = INSERT PLAYER'S SCORE HERE!``````````````````````
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
