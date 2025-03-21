using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace BattleBucks.SyncDash
{
    /// <summary>
    /// manages all in game UI
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Slider slider;
        private void OnEnable()
        {
            EventManager.OnGameOver += ShowGameOverUI;
        }
        private void Start()
        {
            gameOverPanel.SetActive(false);

            restartButton.onClick.AddListener(RestartGame);
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            slider.value = GameManager.Instance.syncDelay;

            // Add listener to call the method when slider value changes
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        // This method is called whenever the slider's value changes
        public void OnSliderValueChanged(float value)
        {
            // Update the floatValue based on the slider value
            GameManager.Instance.syncDelay = value;

            // Optional: You can print the value for debugging
            Debug.Log("Float Value: " + GameManager.Instance.syncDelay);
        }

        // Optional: If you need to remove the listener when the object is destroyed
        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
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
}