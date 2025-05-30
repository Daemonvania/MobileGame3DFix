using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private RectTransform MenuItems;
    
    [SerializeField] Image BG;
    [SerializeField] private RectTransform gameMenu;
    
    RoundManager roundManager;

    private Color bgColor;
    private void Awake()
    {
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
    }
    
    private void Start()
    {
        mainMenu.SetActive(true);
        gameMenu.gameObject.SetActive(false);
        bgColor = BG.material.color;
    }

    public void StartGame()
    {
        MenuItems.DOMove(MenuItems.position + new Vector3(0, -2500, 0), 0.65f)
            .SetEase(Ease.InBack).OnComplete(() =>
            {
                //waitt this insnt 
                    MenuItems.position =
                        new Vector3(MenuItems.position.x, 0, MenuItems.position.z);
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
        gameMenu.position = new Vector3(-500, gameMenu.position.y, gameMenu.transform.position.z);
        gameMenu.transform.DOLocalMoveX(0, 0.65f)
            .SetEase(Ease.OutBack);
    }
    
    public void ReturnToMenu()
    {
        roundManager.ResetGame();
        mainMenu.SetActive(true);
        gameMenu.gameObject.SetActive(false);
    }
    
    public void RestartGame()
    {
        mainMenu.SetActive(false);
        gameMenu.gameObject.SetActive(true);
        roundManager.StartGame();
    }
}
