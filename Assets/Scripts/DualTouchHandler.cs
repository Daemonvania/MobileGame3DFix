using UnityEngine;
using Lean.Touch;
using UnistrokeGestureRecognition.Example;
using UnityEngine.Serialization;

public class DualTouchHandler : MonoBehaviour {
     [SerializeField] ExampleRecognizerController bottomRecognizer;
     [SerializeField] ExampleRecognizerController topRecognizer;

     [SerializeField] DrawCut bottomCutter;
     [SerializeField] DrawCut topCutter;
     
  
     
    private void OnEnable() {
        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUp += HandleFingerUp;
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    private void OnDisable() {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUp -= HandleFingerUp;
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    private void HandleFingerDown(LeanFinger finger)
    {
        if (finger.ScreenPosition.y < Screen.height / 2)
        {
            bottomRecognizer.OnExternalFingerDown(finger);
            bottomCutter.OnExternalFingerDown(finger);
        }
        else
        {
            topRecognizer.OnExternalFingerDown(finger);
            topCutter.OnExternalFingerDown(finger);
        }
    }

    private void HandleFingerUp(LeanFinger finger) {
        if (finger.ScreenPosition.y < Screen.height / 2)
        {
            bottomRecognizer.OnExternalFingerUp(finger);
            bottomCutter.OnExternalFingerUp(finger);
        }
        else
        {
            topRecognizer.OnExternalFingerUp(finger);
            topCutter.OnExternalFingerUp(finger);
        }
    }

    private void HandleFingerUpdate(LeanFinger finger) {
        if (finger.ScreenPosition.y < Screen.height / 2)
        {
            bottomRecognizer.OnExternalFingerSet(finger);
            bottomCutter.OnExternalFingerUpdate(finger);
        }
        else
        {
            topRecognizer.OnExternalFingerSet(finger);
            topCutter.OnExternalFingerUpdate(finger);
        }
    }
}
