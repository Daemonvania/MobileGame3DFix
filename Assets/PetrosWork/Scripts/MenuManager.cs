using DG.Tweening;
using Shapes2D;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private RectTransform MenuItems;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private RectTransform pauseItems;
    
    [SerializeField] private AdsManager adsManager;
    
    [SerializeField] Image BG;
    [SerializeField] private RectTransform gameMenu;

    [SerializeField] private AudioClip buttonClick;
    
    RoundManager roundManager;

    private Color bgColor;
    private void Awake()
    {
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
        pauseMenu.SetActive(false);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    
    private void Start()
    {
        mainMenu.SetActive(true);
        gameMenu.gameObject.SetActive(false);
        bgColor = BG.material.color;
    }

    public void StartGame()
    {
        SoundEffectsManager.Instance.PlaySoundFXClip(buttonClick, transform, 0.5f);
        MenuItems.DOAnchorPos(MenuItems.anchoredPosition + new Vector2(0, -2500), 0.65f)
            .SetEase(Ease.InBack).OnComplete(() =>
            {
                //waitt this insnt 
                    MenuItems.anchoredPosition =
                        new Vector2(MenuItems.anchoredPosition.x, 0);
                    mainMenu.SetActive(false);
                    gameMenu.gameObject.SetActive(true);
                    BG.material.color = new Color(bgColor.r, bgColor.g, bgColor.b, bgColor.a);
                    AnimateIngameMenu();
                    roundManager.StartGame();
            });
        BG.material.DOColor(new Color(bgColor.r, bgColor.g, bgColor.b, 0), 0.5f).SetEase(Ease.InSine);
    }


    void AnimateIngameMenu()
    {
        gameMenu.anchoredPosition = new Vector2(-500, gameMenu.anchoredPosition.y);
        gameMenu.DOAnchorPos(new Vector2(0, 0), 0.65f)
            .SetEase(Ease.OutBack);
    }
    
    public void ReturnToMenu()
    {
        adsManager.ShowInterstitialAd();
        roundManager.ResetGame();
        mainMenu.SetActive(true);
        gameMenu.gameObject.SetActive(false);
        adsManager.ShowBannerAd();
    }
    
    public void RestartGame()
    {
        adsManager.ShowInterstitialAd();
        mainMenu.SetActive(false);
        gameMenu.gameObject.SetActive(true);
        roundManager.StartGame();
    }
    
    public void PauseGame()
    {
        SoundEffectsManager.Instance.PlaySoundFXClip(buttonClick, transform, 0.5f);
        pauseMenu.SetActive(true);
        pauseItems.DOAnchorPos(pauseItems.anchoredPosition + new Vector2(0, -2500), 0.65f)
            .SetEase(Ease.InBack).OnComplete(() =>
            {
                pauseItems.anchoredPosition = new Vector2(pauseItems.anchoredPosition.x, 0);
            });
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        SoundEffectsManager.Instance.PlaySoundFXClip(buttonClick, transform, 0.5f);
        pauseItems.DOAnchorPos(pauseItems.anchoredPosition + new Vector2(0, -2500), 0.65f)
            .SetEase(Ease.InBack).OnComplete(() =>
            {
                pauseMenu.SetActive(false);
                pauseItems.anchoredPosition = new Vector2(pauseItems.anchoredPosition.x, 0);
            });
        Time.timeScale = 1;
    }
}
