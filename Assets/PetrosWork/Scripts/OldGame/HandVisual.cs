using UnityEngine;

public class HandVisual : MonoBehaviour
{
    private Hand hand;
    
    float startPosX;
    float endPosX;

    [SerializeField] float rotateAmount = 30f;

    private float beginRot;
    private float endRot;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hand = GetComponentInParent<Hand>();    
        startPosX = hand.GetPosRange().x;
        endPosX = hand.GetPosRange().y;

        beginRot  = transform.localRotation.eulerAngles.y + rotateAmount;
        endRot = transform.localRotation.eulerAngles.y - rotateAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPlaying) return;

        float handPosX = hand.transform.localPosition.x;

        // Normalize hand position between 0 and 1
        float t = Mathf.InverseLerp(startPosX, endPosX, handPosX);

        // Calculate target rotation using Lerp
        float targetYRotation = Mathf.Lerp(beginRot, endRot, t);

        // Apply new rotation
        Vector3 currentEuler = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(currentEuler.x, targetYRotation, currentEuler.z);
    }

}
