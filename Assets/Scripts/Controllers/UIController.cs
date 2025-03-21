using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private Button restartButton;    
    [SerializeField] private Button mainMenuButton;   

    private void OnEnable()
    {
        EventManager.OnGameOver += ShowGameOverUI;
    }
    private void Start()
    {
        gameOverPanel.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowGameOverUI()
    {
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(1f);
        gameOverPanel.SetActive(true);

    }

    // This method restarts the game
    private void RestartGame()
    {
        gameOverPanel.SetActive(false);
        GameManager.Instance.RestartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ReturnToMainMenu()
    {
        GameManager.Instance.RestartGame();
        SceneManager.LoadScene("MenuScene"); 
    }

    private void OnDisable()
    {
        EventManager.OnGameOver -= ShowGameOverUI;
    }
}
