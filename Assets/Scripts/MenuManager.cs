using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameMenu;
    
    RoundManager roundManager;
    
    private void Awake()
    {
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
    }
    
    private void Start()
    {
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
    }
    
    public void StartGame()
    {
        mainMenu.SetActive(false);
        gameMenu.SetActive(true);
        roundManager.StartGame();
    }
    
    public void ReturnToMenu()
    {
        roundManager.ResetGame();
        mainMenu.SetActive(true);
        gameMenu.SetActive(false);
    }
    
    public void RestartGame()
    {
        mainMenu.SetActive(false);
        gameMenu.SetActive(true);
        roundManager.StartGame();
    }
}
