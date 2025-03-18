using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public static PlatformController instance { get; private set; }
    public Platform platformPrefab;   // Platform prefab to instantiate
    public float platformLength = 25f;  // Length of each platform segment
    public float minSpeed = 5f;         // Minimum speed of platform movement
    public float maxSpeed = 15f;        // Maximum speed after acceleration
    public int maxSegments = 10;        // Number of active platform segments
    public float acceleration = 0.2f;   // How fast the speed increases over time

    private float currentSpeed;         // Current speed of platform movement
    private List<Platform> activePlatforms = new List<Platform>();  // List of active platform segments
    private Queue<Platform> platformPool = new Queue<Platform>();   // Object pool of platform segments
    public Transform playerTransform;  // Reference to the player's transform

    private float recycleDistance = -30f;   // Distance behind the player where platforms get recycled

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializePlatformPool();
        currentSpeed = minSpeed;
        SpawnInitialPlatforms();
    }

    void Update()
    {
        if (GameManager.Instance.GameOver)
            return;
        MovePlatforms();
        HandleSpeedIncrease();
        RecycleOutOfViewPlatforms();
    }

    // Initialize the platform pool
    void InitializePlatformPool()
    {
        for (int i = 0; i < maxSegments; i++)
        {
            Platform platform = Instantiate(platformPrefab);
            platform.gameObject.SetActive(false);
            platformPool.Enqueue(platform);
        }
    }

    // Spawn the initial platforms
    void SpawnInitialPlatforms()
    {
        for (int i = 0; i < maxSegments; i++)
        {
            Vector3 spawnPosition = new Vector3(playerTransform.position.x, 0, i * platformLength);
            SpawnPlatform(spawnPosition);
        }
    }

    // Move all active platforms backward relative to the player
    void MovePlatforms()
    {
        float movementDistance = currentSpeed * Time.deltaTime;

        foreach (Platform platform in activePlatforms)
        {
            platform.transform.Translate(Vector3.back * movementDistance);
        }
    }

    // Handle speed increase over time
    void HandleSpeedIncrease()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
    }

    // Recycle platforms when they move behind the player
    void RecycleOutOfViewPlatforms()
    {
        if (activePlatforms.Count > 0)
        {
            Platform firstPlatform = activePlatforms[0];

            // Check if the platform has moved behind the player
            if (firstPlatform.transform.position.z < playerTransform.position.z + recycleDistance)
            {
                RecyclePlatform();
            }
        }
    }

    // Recycle the platform by moving it to the front
    void RecyclePlatform()
    {
        Platform oldPlatform = activePlatforms[0];
        activePlatforms.RemoveAt(0);
        platformPool.Enqueue(oldPlatform);
        oldPlatform.gameObject.SetActive(false);

        // Move the platform to the front
        Vector3 newPosition = activePlatforms[activePlatforms.Count - 1].transform.position + Vector3.forward * platformLength;
        SpawnPlatform(newPosition);
    }

    // Spawn or reuse a platform from the pool
    void SpawnPlatform(Vector3 position)
    {
        Platform platform = platformPool.Dequeue();
        platform.transform.position = position;
        platform.AddObstaclesAndCollectibles();
        platform.gameObject.SetActive(true);
        activePlatforms.Add(platform);
    }

    public float GetSpeedRatio()
    {
        return currentSpeed;
    }
}
