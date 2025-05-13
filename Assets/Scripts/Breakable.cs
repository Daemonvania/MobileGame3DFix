using System;
using UnityEngine;

[SelectionBase]
public class Breakable : MonoBehaviour
{
    
    [SerializeField] private GameObject wholeObject;
    [SerializeField] private GameObject brokenObject;
    [SerializeField] private GameObject[] brokenParts;
    // BoxCollider boxCollider;

    private void OnEnable()
    {
        Hand.OnBrickHit += Break;
        // Hand.OnBrickMiss += Break;
    }
    private void OnDisable()
    {
        Hand.OnBrickHit -= Break;
        // Hand.OnBrickMiss -= Break;
    }

    private void Awake()
    {
        // boxCollider = GetComponent<BoxCollider>();
        Reset();
    }

    public void Break()
    {
        wholeObject.SetActive(false);
        brokenObject.SetActive(true);
        // boxCollider.enabled = false;
    }

    public void Reset()
    {
        foreach (var part in brokenParts)
        {
            part.transform.localPosition = Vector3.zero;
            part.transform.localRotation = Quaternion.identity;
            part.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            part.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            wholeObject.SetActive(true);
            brokenObject.SetActive(false);
        }
    }
}
