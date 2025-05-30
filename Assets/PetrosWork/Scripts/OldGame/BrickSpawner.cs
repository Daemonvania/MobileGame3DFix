using System;
using System.Collections;
using Shapes2D;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
  [SerializeField] Brick[] bricks;
  
  [SerializeField] float hidePiecesDelay = 1f;
  [SerializeField] float spawnNewBrickDelay = 0.5f;

  private float tempDelay1;
  private float tempDelay2;
  
  private int activeBrickIndex = 0;

  //todo move all Start to an OnGameStart event
  
  private void Start()
  {
        tempDelay1 = hidePiecesDelay;
        tempDelay2 = spawnNewBrickDelay;
        hidePiecesDelay = 0;
        spawnNewBrickDelay = 0;
      ShowBrick();
        hidePiecesDelay = tempDelay1;
        spawnNewBrickDelay = tempDelay2;
  }

  private void OnEnable()
  {
        Hand.OnBrickHit += OnHit;
  }
  
  private void OnDisable()
  {
        Hand.OnBrickHit -= OnHit;
  }

  private void OnHit()
  {
      activeBrickIndex++;
      activeBrickIndex %= bricks.Length;
      ShowBrick();
      Debug.Log("OnHit");
  }
  

  void ShowBrick()
  {
      foreach (var brick in bricks)
      {
         if (bricks[activeBrickIndex] != brick)
         {
             StartCoroutine(HideObject(brick.gameObject, hidePiecesDelay));
         }
         else
         {
             StartCoroutine(SpawnBrick(brick, spawnNewBrickDelay));
         }
      }
  }

  private IEnumerator HideObject(GameObject obj, float delay)
  {
      yield return new WaitForSeconds(delay);
      obj.SetActive(false);
  }
  
  private IEnumerator SpawnBrick(Brick brick, float delay)
  {
      yield return new WaitForSeconds(delay);
      brick.gameObject.SetActive(true);
      brick.Reset();
  }
}
