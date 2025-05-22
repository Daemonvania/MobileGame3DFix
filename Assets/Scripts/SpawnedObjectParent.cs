using System;
using UnityEngine;

public class SpawnedObjectParent : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
    }

    private void OnEnable()
    {
        roundManager.onRoundStart += DestroyObjects;
        roundManager.onResetGame += DestroyObjects;
    }

    private void OnDisable()
    {
        roundManager.onRoundStart -= DestroyObjects;
        roundManager.onResetGame -= DestroyObjects;
    }
    
    private void DestroyObjects()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
