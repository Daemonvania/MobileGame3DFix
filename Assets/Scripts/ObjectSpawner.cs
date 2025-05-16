using System;
using System.Collections;
using Shapes2D;
using UnistrokeGestureRecognition.Example;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnedObjectParent;
    [Space]
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private Transform[] spawnPoints;
    
    
    [FloatRange(0f, 10f)]
    public Vector2 timeToSpawnRange = new Vector2(1f, 3f);
    
    private RoundManager roundManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        roundManager = GetComponent<RoundManager>();
    }

    private void OnEnable()
    {
        roundManager.onRoundStart += OnRoundStart;
        roundManager.onRoundEnd += OnRoundEnd;
    }
    private void OnDisable()
    {
        roundManager.onRoundStart -= OnRoundStart;
        roundManager.onRoundEnd -= OnRoundEnd;
    }

    private void OnRoundStart()
    {
        GameObject selectedObject = objectsToSpawn[UnityEngine.Random.Range(0, objectsToSpawn.Length)];
        float timeToSpawn = UnityEngine.Random.Range(timeToSpawnRange.x, timeToSpawnRange.y);
        StartCoroutine(SpawnObject(selectedObject, timeToSpawn));
    }

    private IEnumerator SpawnObject(GameObject selectedObject, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        foreach (var spawnpoint in spawnPoints)
        {
            Instantiate(selectedObject, spawnpoint.position, Quaternion.identity, spawnedObjectParent);
        }
    }
    private void OnRoundEnd(ExampleRecognizerController.ScreenHalf screen)
    {
        
    }

 
    
}
