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
    public delegate void SliceFailed(ExampleRecognizerController.ScreenHalf screen);
    
    [SerializeField] private CutSO[] cutSOs;
    
    [SerializeField] private ExampleRecognizerController[] patternRecognizers;
    
    [SerializeField] private float timeBetweenRounds = 2f;
    
    public event RoundStart onRoundStart;
    public event RoundEnd onRoundEnd;
    public event SliceFailed onSliceFailed;

    private bool canCut = false;
    private bool topCanCut = false;
    private bool bottomCanCut = false;
    
    private CutSO selectedCutSO;
    
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
        int randomIndex = Random.Range(0, cutSOs.Length);
        foreach (var recognizer in patternRecognizers)
        {
            recognizer.SetPattern(cutSOs[randomIndex].pattern);
        }
        selectedCutSO = cutSOs[randomIndex];
        onRoundStart?.Invoke();
    }
    
    public void SetCanCut(bool canCut)
    {
        this.canCut = canCut;
        topCanCut = canCut;
        bottomCanCut = canCut;
    }
    
    void RoundEnded()
    {
        canCut = false;
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
    
    private void OnSliceCompleted(ExampleRecognizerController.ScreenHalf screen, bool isCorrect)
    {
        //todo maybe remove canCut cause I have the split ones
        if (!canCut) return;
            if (screen == ExampleRecognizerController.ScreenHalf.top && topCanCut)
            {
                if (isCorrect)
                {
                    onRoundEnd?.Invoke(ExampleRecognizerController.ScreenHalf.top);
                    RoundEnded();
                }
                else
                {
                    topCanCut = false;
                    onSliceFailed?.Invoke(ExampleRecognizerController.ScreenHalf.top);
                }
            }
            else if (screen == ExampleRecognizerController.ScreenHalf.bottom && bottomCanCut)
            {
                if (isCorrect)
                {
                    onRoundEnd?.Invoke(ExampleRecognizerController.ScreenHalf.bottom);
                    RoundEnded();
                }
                else
                {
                    bottomCanCut = false;
                    onSliceFailed?.Invoke(ExampleRecognizerController.ScreenHalf.bottom);
                }
            }
            
            if (!topCanCut && !bottomCanCut)
            {
                onRoundEnd?.Invoke(ExampleRecognizerController.ScreenHalf.none);
                RoundEnded();
            }
    }

    public CutSO GetSelectedCutSO()
    {
        return selectedCutSO;
    }
}
