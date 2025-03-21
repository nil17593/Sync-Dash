using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; set; }
    [SerializeField] private TextMeshProUGUI scoreText;  // UI Text to display the score
    [SerializeField] private Transform playerTransform;

    private float totalDistanceScore = 0f;     // Cumulative distance score
    private float distanceSinceLastReset = 0f; // Distance since the last player reset
    private int collectiblesScore = 0;
    private float scoreMultiplier = 1f;  // Score multiplier for distance

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        EventManager.OnWorldReset += AddDistanceOnReset;
    }
    private void Start()
    {
        UpdateScoreUI();  // Initialize score display
    }

    public void UpdateScore(Transform player)
    {
        distanceSinceLastReset = player.position.z;
        UpdateScoreUI();
    }
    public void AddDistanceOnReset(float a)
    {
        // Add the current distance to the total score before resetting
        totalDistanceScore += distanceSinceLastReset * scoreMultiplier;
        distanceSinceLastReset = 0f;  // Reset the local distance tracker
    }
    public void AddCollectibleScore(int points)
    {
        collectiblesScore += points;
        UpdateScoreUI();
    }

    // Update the score on the UI
    private void UpdateScoreUI()
    {
        float totalScore = totalDistanceScore + (distanceSinceLastReset * scoreMultiplier) + collectiblesScore;
        scoreText.text = "Score: " + totalScore.ToString("0");  // Display total score (no decimals)
    }

    // Reset the score (for game restarts)
    public void ResetScore()
    {
        collectiblesScore = 0;
        UpdateScoreUI();
    }

    private void OnDisable()
    {
        EventManager.OnWorldReset -= AddDistanceOnReset;
    }
}
