using System;
using System.Collections;
using UnityEngine;

public class SpawnedObjectParent : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private Material transparentFadeMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
    }

    private void OnEnable()
    {
        roundManager.onRoundStart += DestroyObjects;
        roundManager.onResetGame += DestroyObjects;
    }

    private void OnDisable()
    {
        roundManager.onRoundStart -= DestroyObjects;
        roundManager.onResetGame -= DestroyObjects;
    }
    
    private void DestroyObjects()
    {
        foreach (Transform obj in transform)
        {
            MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                Debug.Log("Destroying object: " + obj.name);
                StartCoroutine(FadeOutAndDestroy(obj.gameObject, renderer));
            }
        }
    }

    private IEnumerator FadeOutAndDestroy(GameObject obj, MeshRenderer renderer, float duration = 1.0f)
    {
        // Assign a unique instance of the fade material
        Material fadeMat = new Material(transparentFadeMaterial); // clone it
        renderer.material = fadeMat;

        Color originalColor = fadeMat.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / duration);
            fadeMat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(obj);
    }


    
    // Update is called once per frame
    void Update()
    {
        
    }
}
