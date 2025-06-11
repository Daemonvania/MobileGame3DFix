using System;
using System.Collections;
using System.Threading.Tasks;
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
    
    private async void DestroyObjects()
    {
        foreach (Transform obj in transform)
        {
            foreach (MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer != null)
                {
                    StartCoroutine(FadeOut(renderer.gameObject, renderer));
                }
            }
        }
        await Task.Delay(1000); // Wait for the fade out to complete
        foreach (Transform obj in transform)
        {
            Destroy(obj.gameObject);
        }
    }

    private IEnumerator FadeOut(GameObject obj, MeshRenderer renderer, float duration = 1.0f)
    {
        // Assign a unique instance of the fade material
        Material fadeMat = new Material(transparentFadeMaterial); // clone it
        renderer.materials = new Material[] { fadeMat }; // set the material to the renderer
        // renderer.material = fadeMat;

        Color originalColor = fadeMat.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / duration);
            fadeMat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }


    
    // Update is called once per frame
    void Update()
    {
        
    }
}
