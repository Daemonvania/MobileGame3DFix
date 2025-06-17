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
        Material[] materials = renderer.materials; // Creates copies for editing

        // Prepare materials for transparency
        foreach (Material fadeMat in materials)
        {
            Debug.Log("Fading out: " + fadeMat.name);

            fadeMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            fadeMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            fadeMat.SetInt("_ZWrite", 0);
            fadeMat.SetInt("_Surface", 1);

            fadeMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            fadeMat.SetShaderPassEnabled("DepthOnly", false);
            fadeMat.SetShaderPassEnabled("SHADOWCASTER", false);

            fadeMat.SetOverrideTag("RenderType", "Transparent");

            fadeMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            fadeMat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float elapsed = 0f;

        // Get original colors
        Color[] originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_Color"))
                originalColors[i] = materials[i].color;
            else
                Debug.LogWarning($"Material {materials[i].name} has no _Color property.");
        }

        // Fade all materials in one loop
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            for (int i = 0; i < materials.Length; i++)
            {
                Color c = originalColors[i];
                materials[i].color = new Color(c.r, c.g, c.b, Mathf.Lerp(c.a, 0f, t));
            }

            yield return null;
        }
    }


    
    // Update is called once per frame
    void Update()
    {
        
    }
}
