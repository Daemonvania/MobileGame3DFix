using System;
using DG.Tweening;
using UnistrokeGestureRecognition.Example;
using UnityEngine;
using UnityEngine.UI;

public class ManageScore : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;
    
    //todo split UI manager
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
    
    [SerializeField] private Sprite p1Dot;
    [SerializeField] private Sprite p2Dot;
    [SerializeField] private Sprite darkDot;
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
        p1Checkmark.preserveAspect = true;
        p2Checkmark.preserveAspect = true;
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
    
        p1Checkmark.DOFade(0, 0.3f).OnComplete(() => 
        {
            p1Checkmark.gameObject.SetActive(false);
        });
        p2Checkmark.DOFade(0, 0.3f).OnComplete(() => 
        {
            p2Checkmark.gameObject.SetActive(false);
        });
    }
    void OnRoundEnd(ExampleRecognizerController.ScreenHalf screen)
    {
        if (screen == ExampleRecognizerController.ScreenHalf.top)
        {
            player1ScoreSprites[player1Score].sprite = p1Dot;
            player1ScoreSprites[player1Score].rectTransform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.6f, 5, 0.25f);
            player1Score++;
            p1Checkmark.sprite = checkMark;
            p2Checkmark.sprite = cross;
        }
        else if (screen == ExampleRecognizerController.ScreenHalf.bottom)
        {
            player2ScoreSprites[player2Score].sprite = p2Dot;
            player2ScoreSprites[player2Score].rectTransform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.6f, 5, 0.25f);

            player2Score++;
            p2Checkmark.sprite = checkMark;
            p1Checkmark.sprite = cross;
        }
        else
        {
            p1Checkmark.sprite = cross;
            p2Checkmark.sprite = cross;
        }
        AnimateCheckmark(p1Checkmark, true);
        AnimateCheckmark(p2Checkmark, false);
        
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
            icon.sprite = darkDot;
        }   
        foreach (var icon in player2ScoreSprites)
        {
            icon.sprite = darkDot;
        }
        p1Victory.SetActive(false);
        p2Victory.SetActive(false);
        p1Defeat.SetActive(false);
        p2Defeat.SetActive(false);
    }
    
    
    private void FailedCut(ExampleRecognizerController.ScreenHalf screen)
    {
        //refactor so code isnt repeated
      
        if (screen == ExampleRecognizerController.ScreenHalf.top)
        {
            p1Checkmark.sprite = cross;
            AnimateCheckmark(p1Checkmark, true);
        }
        else if (screen == ExampleRecognizerController.ScreenHalf.bottom)
        {
            p2Checkmark.sprite = cross;
            AnimateCheckmark(p2Checkmark, false);
        }
    }
    
    private void AnimateCheckmark(Image checkmark, bool isTopScreen)
    {
        //todo perhaps if it already appeared dont play it 
        
        checkmark.gameObject.SetActive(true);
        checkmark.DOFade(1, 0.1f);
        
        int direction = isTopScreen ? 1 : -1;
        
        checkmark.rectTransform.anchoredPosition = new Vector3(0, direction * 100, 0);
        checkmark.rectTransform.DOAnchorPos(new Vector2(0, -direction * 153), 0.6f).SetEase(Ease.OutBack);
        Debug.Log("AnimateCheckmark");
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
