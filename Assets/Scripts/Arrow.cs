using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private GameObject sliceMesh;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position  = new Vector3(sliceMesh.transform.position.x, sliceMesh.transform.position.y + 0.50f, sliceMesh.transform.position.z -0.4f);
    }
}
