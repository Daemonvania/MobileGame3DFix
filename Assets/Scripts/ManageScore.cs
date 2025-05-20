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

    [SerializeField] private Image p1Checkmark;
    [SerializeField] private Image p2Checkmark;
    [Space]
    [SerializeField] private Sprite checkMark ;
    [SerializeField] private Sprite cross;
    [Space]
    //todo populate the score sprites from here..?
    [SerializeField] int scoreToWin = 6;
    
    private int player1Score = 0;
    private int player2Score = 0;


    private void OnEnable()
    {
        roundManager.onRoundStart += OnRoundStart;
        roundManager.onRoundEnd += OnRoundEnd;
        roundManager.onSliceFailed += FailedCut;
    }
    private void OnDisable()
    {
        roundManager.onRoundStart -= OnRoundStart;
        roundManager.onRoundEnd -= OnRoundEnd;
        roundManager.onSliceFailed -= FailedCut;
    }

    private void OnRoundStart()
    {
        p1Checkmark.gameObject.SetActive(false);
        p2Checkmark.gameObject.SetActive(false);
    }
    void OnRoundEnd(ExampleRecognizerController.ScreenHalf screen)
    {
        p1Checkmark.gameObject.SetActive(true);
        p2Checkmark.gameObject.SetActive(true);
        if (screen == ExampleRecognizerController.ScreenHalf.top)
        {
            player1ScoreSprites[player1Score].color = Color.blue;
            player1Score++;
            p1Checkmark.sprite = checkMark;
            p2Checkmark.sprite = cross;
        }
        else if (screen == ExampleRecognizerController.ScreenHalf.bottom)
        {
            player2ScoreSprites[player2Score].color = Color.red;
            player2Score++;
            p2Checkmark.sprite = checkMark;
            p1Checkmark.sprite = cross;
        }
        else
        {
            p1Checkmark.sprite = cross;
            p2Checkmark.sprite = cross;
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

    private void FailedCut(ExampleRecognizerController.ScreenHalf screen)
    {
        if (screen == ExampleRecognizerController.ScreenHalf.top)
        {
            p1Checkmark.gameObject.SetActive(true);
            p1Checkmark.sprite = cross;
        }
        else if (screen == ExampleRecognizerController.ScreenHalf.bottom)
        {
            p2Checkmark.gameObject.SetActive(true);
            p2Checkmark.sprite = cross;
        }
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
