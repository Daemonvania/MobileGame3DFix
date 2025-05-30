using System;
using System.Collections;
using Shapes2D;
using UnistrokeGestureRecognition.Example;
using UnityEditor;
using UnityEngine;

public class FloatRangeAttribute : PropertyAttribute
{
    public float Min;
    public float Max;

    public FloatRangeAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }
}
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
            GameObject spawnedObject = Instantiate(selectedObject, spawnpoint.position, Quaternion.identity, spawnedObjectParent);
            if (spawnPoints[0] == spawnpoint)
            {
                spawnedObject.GetComponentInChildren<Arrow>().SetArrowImage(roundManager.GetSelectedCutSO().gestureImage, roundManager, ExampleRecognizerController.ScreenHalf.top);
            }
            else
            {
                spawnedObject.GetComponentInChildren<Arrow>().SetArrowImage(roundManager.GetSelectedCutSO().gestureImage, roundManager, ExampleRecognizerController.ScreenHalf.bottom);
            }
        }

        StartCoroutine(EnableCutting(0.7f));
    }
    
    private IEnumerator EnableCutting(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        roundManager.SetCanCut(true);
    }
    
    private void OnRoundEnd(ExampleRecognizerController.ScreenHalf screen)
    {
        
    }

 
    
}
