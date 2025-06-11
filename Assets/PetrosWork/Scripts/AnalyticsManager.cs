using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;
    private bool _isInitialized = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitialized = true;
    }
    public void GameCompleted(int player1Score, int player2Score, string winner)
    {
        if (!_isInitialized) return;

        CustomEvent endGameEvent = new CustomEvent("game_completed")
        {
            { "player1_score", player1Score },
            { "player2_score", player2Score },
            { "game_winner", winner }
        };
        AnalyticsService.Instance.RecordEvent(endGameEvent);
        AnalyticsService.Instance.Flush();
    }
    
    
}
