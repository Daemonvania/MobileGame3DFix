using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Brick : MonoBehaviour
{
    [SerializeField] private Transform hitPoint;
    [Space] 
    
    private Breakable breakable;
    
    Vector3 startPos;
    private Vector3 offScreenPos;
    private void Awake()
    {
        breakable = GetComponent<Breakable>();
        startPos = transform.position;
        offScreenPos = new Vector3(startPos.x-1.5f, startPos.y, startPos.z);
    }

    public void Reset()
    {
        breakable.Reset();
        transform.position = offScreenPos;
        transform.DOMove(startPos, 0.5f);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
