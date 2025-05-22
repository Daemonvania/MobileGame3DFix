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
    [SerializeField] private GameObject p1Victory;
    [SerializeField] private GameObject p2Victory;
    
    [SerializeField] private GameObject p1Defeat;
    [SerializeField] private GameObject p2Defeat;
    
    [Space]
    [SerializeField] int scoreToWin = 6;
    
    private int player1Score = 0;
    private int player2Score = 0;


    private void Start()
    {
        p1Victory.SetActive(false);
        p2Victory.SetActive(false);
        p1Defeat.SetActive(false);
        p2Defeat.SetActive(false);
        p1Checkmark.gameObject.SetActive(false);
        p2Checkmark.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        roundManager.onRoundStart += OnRoundStart;
        roundManager.onRoundEnd += OnRoundEnd;
        roundManager.onSliceFailed += FailedCut;
        roundManager.onResetGame += ResetScore;
    }
    private void OnDisable()
    {
        roundManager.onRoundStart -= OnRoundStart;
        roundManager.onRoundEnd -= OnRoundEnd;
        roundManager.onSliceFailed -= FailedCut;
        roundManager.onResetGame -= ResetScore;
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
            ShowVictoryScreen(ExampleRecognizerController.ScreenHalf.top);
        }
        else if (player2Score >= scoreToWin)
        {
            ShowVictoryScreen(ExampleRecognizerController.ScreenHalf.bottom);
        }
    }

    void ShowVictoryScreen(ExampleRecognizerController.ScreenHalf screen)
    {
        roundManager.StopGame();
        
        roundManager.SetCanCut(false);
        p1Checkmark.gameObject.SetActive(false);
        p2Checkmark.gameObject.SetActive(false);
        if (screen == ExampleRecognizerController.ScreenHalf.top)
        {
            p1Victory.SetActive(true);
            p2Defeat.SetActive(true);
        }
        else if (screen == ExampleRecognizerController.ScreenHalf.bottom)
        {
            p2Victory.SetActive(true);
            p1Defeat.SetActive(true);
        }
        
    }

    private void ResetScore()
    {
        player1Score = 0;
        player2Score = 0;
        foreach (var icon in player1ScoreSprites)
        {
            icon.color = Color.white;
        }   
        foreach (var icon in player2ScoreSprites)
        {
            icon.color = Color.white;
        }
        p1Victory.SetActive(false);
        p2Victory.SetActive(false);
        p1Defeat.SetActive(false);
        p2Defeat.SetActive(false);
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
