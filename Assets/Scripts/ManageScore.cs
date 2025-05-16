using System;
using UnistrokeGestureRecognition.Example;
using UnityEngine;

public class ManageScore : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;
    
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
            player1Score++;
        }
        else
        {
            player2Score++;
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
