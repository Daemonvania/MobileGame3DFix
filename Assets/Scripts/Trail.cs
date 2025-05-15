using UnityEngine;
using Lean.Touch;

public class Trail : MonoBehaviour
{
    [SerializeField] Camera camera;
    public void OnExternalFingerUpdate(LeanFinger finger)
    {
        Vector3 screenPos = finger.ScreenPosition;
        screenPos.z = 0.5f; // Distance from the camera, same as parameter in GetWorldPosition
        transform.position = camera.ScreenToWorldPoint(screenPos);

    }
}
