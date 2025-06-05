using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Lean.Touch;

public class DrawCut : MonoBehaviour
{
    Vector3 pointA;
    Vector3 pointB;
    
    
    private LineRenderer cutRender;
    private bool animateCut;

    private Camera cam;

    private Coroutine cutCoroutine;
    
    private Vector3 lastWorldPos;
    private bool hasLastWorldPos;

    Transform spawnedObjectParent;
    
    [SerializeField] private AudioClip cutSound;
    private void Awake()
    {
        spawnedObjectParent = GameObject.FindGameObjectWithTag("SpawnedObjectParent").transform;
    }

   

    void Start()
    {
        cam = GetComponent<Camera>();
        cutRender = GetComponent<LineRenderer>();
        cutRender.startWidth = 0.05f;
        cutRender.endWidth = 0.05f;

        // LeanTouch.OnFingerDown += HandleFingerDown;
        // LeanTouch.OnFingerUpdate += HandleFingerUpdate;
        // LeanTouch.OnFingerUp += HandleFingerUp;
    }

    void OnDestroy()
    {
        // LeanTouch.OnFingerDown -= HandleFingerDown;
        // LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
        // LeanTouch.OnFingerUp -= HandleFingerUp;
    }

    public void OnExternalFingerDown(LeanFinger finger)
    {
        Vector3 screenPos = finger.ScreenPosition;
        screenPos.z = -cam.transform.position.z;
        pointA = cam.ScreenToWorldPoint(screenPos);
        
        lastWorldPos = pointA;
        hasLastWorldPos = true;
        // cutCoroutine = StartCoroutine(CutCoroutine(finger));
        
    }

    public void OnExternalFingerUpdate(LeanFinger finger)
    {
        animateCut = false;

        Vector3 screenPos = finger.ScreenPosition;
        screenPos.z = -cam.transform.position.z;
        Vector3 currentPoint = cam.ScreenToWorldPoint(screenPos);

        cutRender.positionCount = 2;
        cutRender.SetPosition(0, pointA);
        cutRender.SetPosition(1, currentPoint);
        cutRender.startColor = Color.gray;
        cutRender.endColor = Color.gray;
        
        // Direction change detection
        if (hasLastWorldPos)
        {
            Vector3 prevDir = (lastWorldPos - pointA).normalized;
            Vector3 currentDir = (currentPoint - lastWorldPos).normalized;

            if (prevDir != Vector3.zero && currentDir != Vector3.zero)
            {
                float angle = Vector3.Angle(prevDir, currentDir);
                if (angle > 70f)
                {
                    OnExternalFingerUp(finger);
                    OnExternalFingerDown(finger);
                }
            }
        }
        lastWorldPos = currentPoint;
    }

    public void OnExternalFingerUp(LeanFinger finger)
    {
    
        Vector3 screenPos = finger.ScreenPosition;
        screenPos.z = -cam.transform.position.z;
        pointB = cam.ScreenToWorldPoint(screenPos);

        CreateSlicePlane();

        // if (cutCoroutine != null)
        // {
        //     StopCoroutine(cutCoroutine);
        //     cutCoroutine = null;
        // }
        // else
        // {
        //     OnExternalFingerDown(finger);
        // }
        
        cutRender.positionCount = 2;
        cutRender.SetPosition(0, pointA);
        cutRender.SetPosition(1, pointB);
        animateCut = true;
    }

    void Update()
    {
        if (animateCut)
        {
            cutRender.SetPosition(0, Vector3.Lerp(pointA, pointB, 1f));
        }
    }

    // private IEnumerator CutCoroutine(LeanFinger finger)
    // {
    //     yield return new WaitForSeconds(CutDelay); 
    //     cutCoroutine = null;
    //    OnExternalFingerUp(finger);
    // }
    void CreateSlicePlane()
    {
        Vector3 pointInPlane = (pointA + pointB) / 2;
        Vector3 cutPlaneNormal = Vector3.Cross((pointA - pointB), (pointA - cam.transform.position)).normalized;
        Quaternion orientation = Quaternion.FromToRotation(Vector3.up, cutPlaneNormal);

        var all = Physics.OverlapBox(pointInPlane, new Vector3(10, 0.01f, 10), orientation);

        foreach (var hit in all)
        {
            if (hit.gameObject.CompareTag("CutItem"))
            {
                SoundEffectsManager.Instance.PlaySoundFXClip(
                    cutSound, transform, 1f);
                MeshFilter filter = hit.gameObject.GetComponentInChildren<MeshFilter>();
                if (filter != null)
                {
                    Cutter.Cut(hit.gameObject, pointInPlane, cutPlaneNormal, spawnedObjectParent);
                }
            }
        }
    }
}
