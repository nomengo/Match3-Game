using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private Board board;
    public GameObject LowPanel;
    public Text TimeText;
    public Text ScoreText;
    private float currentTime;
    public float startingTime;
    
    void Start()
    {
        board = FindObjectOfType<Board>();
        currentTime = startingTime;
        LowPanel.SetActive(false);
    }
    void Update()
    {
        if (currentTime >= 0)
        {
            currentTime -= 1 * Time.deltaTime;
            TimeText.text = "Time: " + currentTime.ToString("0");
            
        }
        else
        {
            board.GetComponent<Board>().currentState = GameState.GameOver;
            PanelManagement();
        }

        if (board.GetComponent<Board>().currentState != GameState.GameOver)
        {
            ScoreText.text = "Score: " + board.GetComponent<Board>().dotCount.ToString();
        }
    }
    void PanelManagement()
    {
        LowPanel.SetActive(true);
    }
}
