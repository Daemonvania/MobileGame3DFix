using UnityEngine;

public class ArrowVisual : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 20f;
    
    
    Transform hitPointTransform;
    HitPoint hitPoint;
    
    [SerializeField] float rotateAmount = 30f;

    private float beginRot;
    private float endRot;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitPoint = GetComponentInParent<HitPoint>();
        hitPointTransform = hitPoint.gameObject.transform;
        
        beginRot  = transform.localRotation.eulerAngles.y + rotateAmount;
        endRot = transform.localRotation.eulerAngles.y - rotateAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsPlaying == false) return;

        float handPosX = hitPointTransform.position.x;

        // Normalize hand position between 0 and 1
        float t = Mathf.InverseLerp(hitPoint.brickStart.transform.position.x, hitPoint.brickEnd.transform.position.x, handPosX);

        // Calculate target rotation using Lerp
        float targetYRotation = Mathf.Lerp(beginRot, endRot, t);

        // Apply new rotation
        Vector3 currentEuler = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(currentEuler.x, targetYRotation, currentEuler.z);
      
    }
}
