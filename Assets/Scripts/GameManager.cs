using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Hand hand;
    [SerializeField] private Brick brick;
    
    public static readonly bool IsPlaying = true;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         Debug.Log(IsPlaying);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //todo possibility of moving a lot of general functionality here, right now its all done in individual objects listening on the observer
    private void OnHit()
    {
        StartCoroutine(hand.Reset(0.5f));
    }
    
    private void OnMiss()
    {
        StartCoroutine(hand.Reset(0.1f));
    }
}
