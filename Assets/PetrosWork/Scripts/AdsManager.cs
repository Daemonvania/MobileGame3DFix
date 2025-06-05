using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;
    Interstitial interstitial;
    BannerAd bannerAd;


    private void Awake()
    {
        interstitial = GetComponent<Interstitial>();
        bannerAd = GetComponent<BannerAd>();

        
        if (interstitial == null)
        {
            Debug.LogError("Interstitial component not found on AdsManager.");
        }

        if (bannerAd == null)
        {
            Debug.LogError("BannerAd component not found on AdsManager.");
        }
    }


    private void OnEnable()
    {
        roundManager.onResetGame += LoadAd;
    }
    private void OnDisable()
    {
        roundManager.onResetGame -= LoadAd;
    }
    
    private void LoadAd()
    {
        interstitial.LoadAd();
    }
    
    public void ShowInterstitialAd()
    {
        if (interstitial != null)
        {
            interstitial.ShowAd();
        }
        else
        {
            Debug.LogWarning("Interstitial component is not assigned.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
