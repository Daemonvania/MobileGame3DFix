using System;
using UnistrokeGestureRecognition.Example;
using UnityEngine;
using UnityEngine.UI;

public class ManageScore : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;
    
    //todo can do only one array of sprites and use the index 
    [SerializeField] private Image[] player1ScoreSprites;
    [SerializeField] private Image[] player2ScoreSprites;
    [Space]
    
    //todo populate the score sprites from here..?
    [SerializeField] int scoreToWin = 6;
    
    private int player1Score = 0;
    private int player2Score = 0;


    private void OnEnable()
    {
        roundManager.onRoundEnd += OnRoundEnd;
    }
    private void OnDisable()
    {
        roundManager.onRoundEnd -= OnRoundEnd;
    }

    void OnRoundEnd(ExampleRecognizerController.ScreenHalf screen)
    {
        if (screen == ExampleRecognizerController.ScreenHalf.top)
        {
            player1ScoreSprites[player1Score].color = Color.blue;
            player1Score++;
        }
        else
        {
            player2ScoreSprites[player2Score].color = Color.red;
            player2Score++;
        }
        if (player1Score >= scoreToWin)
        {
            Debug.Log("Player 1 wins!");
            // Handle player 1 win
        }
        else if (player2Score >= scoreToWin)
        {
            Debug.Log("Player 2 wins!");
            // Handle player 2 win
        }
        
        Debug.Log($"Player 1 Score: {player1Score}, Player 2 Score: {player2Score}");
    }
    
    public int GetPlayer1Score()
    {
        return player1Score;
    }
    
    public int GetPlayer2Score()
    {
        return player2Score;
    }
}
