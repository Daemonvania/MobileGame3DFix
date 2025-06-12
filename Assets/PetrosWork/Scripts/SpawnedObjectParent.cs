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
        // Material fadeMat = new Material(transparentFadeMaterial); // clone it
        // renderer.materials = new Material[] { fadeMat }; // set the material to the renderer
        
        foreach (Material fadeMat in renderer.materials)
        {
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
    }


    
    // Update is called once per frame
    void Update()
    {
        
    }
}
