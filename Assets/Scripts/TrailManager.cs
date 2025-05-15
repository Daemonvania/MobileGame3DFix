using System;
using UnityEngine;
using Lean.Touch;
public class TrailManager : MonoBehaviour
{
    [SerializeField] Trail topTrail;
    [SerializeField] Trail bottomTrail;
    
    private void Start()
    {
        topTrail.gameObject.SetActive(false);
        bottomTrail.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }
    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUp -= HandleFingerUp;
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }
    private void HandleFingerDown(LeanFinger finger)
    {
        if (finger.ScreenPosition.y < Screen.height / 2)
        {
            bottomTrail.gameObject.SetActive(true);
        }
        else
        {
            topTrail.gameObject.SetActive(true);

        }
    }
    private void HandleFingerUp(LeanFinger finger)
    {
        if (finger.ScreenPosition.y < Screen.height / 2)
        {
            bottomTrail.gameObject.SetActive(false);
        }
        else
        {
            topTrail.gameObject.SetActive(false);
        }
    }
    private void HandleFingerUpdate(LeanFinger finger)
    {
        if (finger.ScreenPosition.y < Screen.height / 2)
        {
            bottomTrail.OnExternalFingerUpdate(finger);
        }
        else
        {
            topTrail.OnExternalFingerUpdate(finger);
        }
    }
}
