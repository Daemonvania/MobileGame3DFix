using UnityEngine;
using Lean.Touch;
using UnistrokeGestureRecognition.Example;
using UnityEngine.Serialization;

public class DualTouchHandler : MonoBehaviour {
     [SerializeField] ExampleRecognizerController bottomRecognizer;
     [SerializeField] ExampleRecognizerController topRecognizer;

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

    private void HandleFingerDown(LeanFinger finger) {
        if (finger.ScreenPosition.y < Screen.height / 2)
            bottomRecognizer.OnExternalFingerDown(finger);
        else
            topRecognizer.OnExternalFingerDown(finger);
    }

    private void HandleFingerUp(LeanFinger finger) {
        if (finger.ScreenPosition.y < Screen.height / 2)
            bottomRecognizer.OnExternalFingerUp(finger);
        else
            topRecognizer.OnExternalFingerUp(finger);
    }

    private void HandleFingerUpdate(LeanFinger finger) {
        if (finger.ScreenPosition.y < Screen.height / 2)
            bottomRecognizer.OnExternalFingerSet(finger);
        else
            topRecognizer.OnExternalFingerSet(finger);
    }
}
