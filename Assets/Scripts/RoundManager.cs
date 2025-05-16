using System;
using System.Collections;
using System.Collections.Generic;
using UnistrokeGestureRecognition.Example;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RoundManager : MonoBehaviour
{
    
    public delegate void RoundStart();
    public delegate void RoundEnd(ExampleRecognizerController.ScreenHalf screen);
    
    [SerializeField] private ExampleGesturePattern[] possiblePatterns;
    
    [SerializeField] private ExampleRecognizerController[] patternRecognizers;
    
    [SerializeField] private float timeBetweenRounds = 2f;
    
    public event RoundStart onRoundStart;
    public event RoundEnd onRoundEnd;

    private bool canCut = false;

    private void Start()
    {
        StartRound();
    }

    private void OnEnable()
    {
        foreach (var recognizer in patternRecognizers)
        {
            recognizer.onSliceCompleted += OnSliceCompleted;
        }
    }
    
    private void OnDisable()
    {
        foreach (var recognizer in patternRecognizers)
        {
            recognizer.onSliceCompleted -= OnSliceCompleted;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void StartRound()
    {
        int randomIndex = Random.Range(0, possiblePatterns.Length);
        foreach (var recognizer in patternRecognizers)
        {
            recognizer.SetPattern(possiblePatterns[randomIndex]);
        }
        canCut = true;
        onRoundStart?.Invoke();
    }
    
    void RoundEnded()
    {
        foreach (var recognizer in patternRecognizers)
        {
            recognizer.SetPattern(null);
        }

        StartCoroutine(WaitForNextRound(timeBetweenRounds));
    }
    private IEnumerator WaitForNextRound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        StartRound();
    }
    
    private void OnSliceCompleted(ExampleRecognizerController.ScreenHalf screen)
    {
        if (!canCut) return;
        canCut = false;
        if (screen == ExampleRecognizerController.ScreenHalf.top)
        {
           onRoundEnd?.Invoke(ExampleRecognizerController.ScreenHalf.top);
        }
        else
        {
           onRoundEnd?.Invoke(ExampleRecognizerController.ScreenHalf.bottom);
        }
        RoundEnded();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
