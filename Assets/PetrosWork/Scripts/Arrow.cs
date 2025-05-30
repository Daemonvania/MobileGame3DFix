
using System.Collections;
using UnistrokeGestureRecognition.Example;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Vector3 = UnityEngine.Vector3;

public class Arrow : MonoBehaviour
{
    [SerializeField] private GameObject sliceMesh;
    [SerializeField] private Image arrowImage;
    
    ExampleRecognizerController.ScreenHalf screenHalf;
    RoundManager roundManager;

    private bool isTracking = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetArrowImage(Sprite sprite, RoundManager manager, ExampleRecognizerController.ScreenHalf screen)
    {
        arrowImage.sprite = sprite;
        screenHalf = screen;
        roundManager = manager;
        roundManager.onSliceFailed += FailedCut;
        roundManager.onRoundEnd += OnRoundEnd;
        StartCoroutine(WaitToStopTracking());
    }

    private void OnDisable()
    {
        roundManager.onSliceFailed -= FailedCut;
        roundManager.onRoundEnd -= OnRoundEnd;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isTracking) {return;}
        MeshFilter meshFilter = sliceMesh.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            // Local center of the mesh
            Vector3 localCenter = meshFilter.mesh.bounds.center;

            // Convert to world space
            Vector3 worldCenter = sliceMesh.transform.TransformPoint(localCenter);

            // Set this object's position to the world center
            transform.position = worldCenter;
        }    }

    void FailedCut(ExampleRecognizerController.ScreenHalf screen)
    {
          if (screen == screenHalf)
          {
              arrowImage.gameObject.SetActive(false);
          }
    }

    void OnRoundEnd(ExampleRecognizerController.ScreenHalf screen)
    {
            arrowImage.gameObject.SetActive(false);
    }

    private IEnumerator WaitToStopTracking()
    {
        yield return new WaitForSeconds(1.5f);
        isTracking = false;
    }
}
