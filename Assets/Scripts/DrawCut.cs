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

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        cutRender = GetComponent<LineRenderer>();
        cutRender.startWidth = 0.05f;
        cutRender.endWidth = 0.05f;

        LeanTouch.OnFingerDown += HandleFingerDown;
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
        LeanTouch.OnFingerUp += HandleFingerUp;
    }

    void OnDestroy()
    {
        LeanTouch.OnFingerDown -= HandleFingerDown;
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
        LeanTouch.OnFingerUp -= HandleFingerUp;
    }

    private void HandleFingerDown(LeanFinger finger)
    {
        Vector3 screenPos = finger.ScreenPosition;
        screenPos.z = -cam.transform.position.z;
        pointA = cam.ScreenToWorldPoint(screenPos);
    }

    private void HandleFingerUpdate(LeanFinger finger)
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
    }

    private void HandleFingerUp(LeanFinger finger)
    {
        Vector3 screenPos = finger.ScreenPosition;
        screenPos.z = -cam.transform.position.z;
        pointB = cam.ScreenToWorldPoint(screenPos);

        CreateSlicePlane();

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

    void CreateSlicePlane()
    {
        Vector3 pointInPlane = (pointA + pointB) / 2;
        Vector3 cutPlaneNormal = Vector3.Cross((pointA - pointB), (pointA - cam.transform.position)).normalized;
        Quaternion orientation = Quaternion.FromToRotation(Vector3.up, cutPlaneNormal);

        var all = Physics.OverlapBox(pointInPlane, new Vector3(100, 0.01f, 100), orientation);

        foreach (var hit in all)
        {
            MeshFilter filter = hit.gameObject.GetComponentInChildren<MeshFilter>();
            if (filter != null)
            {
                Cutter.Cut(hit.gameObject, pointInPlane, cutPlaneNormal);
            }
        }
    }
}
