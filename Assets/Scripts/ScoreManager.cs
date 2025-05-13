using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    int score = 0;
    int consecutiveHits = 0;
    
    private void OnEnable()
    {
        Hand.OnBrickHit += OnHit;
        Hand.OnBrickMiss += OnMiss;
    }
    
    private void OnDisable()
    {
        Hand.OnBrickHit -= OnHit;
        Hand.OnBrickMiss -= OnMiss;
    }

    private void Start()
    {
        scoreText.text = "Score: " + score;
    }

    private void OnHit()
    {
        Debug.Log(Hand.IsStrong());
        if (Hand.IsStrong())
        {
            score += 2 + consecutiveHits;
            consecutiveHits++;
        }
        else
        {
            score++;
            consecutiveHits = 0;
        }
        scoreText.text = "Score: " + score;
    }
    private void OnMiss()
    {
        score = 0;
        scoreText.text = "Score: " + score;
    }
}
