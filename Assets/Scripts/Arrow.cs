using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Arrow : MonoBehaviour
{
    [SerializeField] private GameObject sliceMesh;
    [SerializeField] private Image arrowImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetArrowImage(Sprite sprite)
    {
        arrowImage.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position  = new Vector3(sliceMesh.transform.position.x, sliceMesh.transform.position.y + 0.50f, sliceMesh.transform.position.z -0.4f);
    }
}
