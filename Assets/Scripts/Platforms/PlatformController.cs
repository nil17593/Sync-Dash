using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleBucks.SyncDash
{
    /// <summary>
    /// controlls all the paltform management recycling the platforms
    /// </summary>
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private Platform platformPrefab;
        [SerializeField] private float platformLength = 25f;
        [SerializeField] private float playerXPos;
        [SerializeField] private int maxSegments = 10;
        [SerializeField] private Transform playerTransform;
        private List<Platform> activePlatforms = new List<Platform>();
        private Queue<Platform> platformPool = new Queue<Platform>();

        void Start()
        {
            InitializePlatformPool();
            SpawnInitialPlatforms();
        }
        void SpawnInitialPlatforms()
        {
            for (int i = 0; i < maxSegments; i++)
            {
                Vector3 spawnPosition = new Vector3(playerTransform.position.x, 0, i * platformLength);

                if (i < 1)
                {
                    SpawnPlatformWithoutObstacles(spawnPosition);
                }
                else
                {
                    SpawnPlatform(spawnPosition);  // Use the normal spawning logic after the first two platforms
                }
            }
        }

        void SpawnPlatformWithoutObstacles(Vector3 position)
        {
            Platform platform = platformPool.Dequeue();
            platform.transform.position = position;
            platform.Cleanup(); // Ensure no obstacles or collectibles are activated
            platform.gameObject.SetActive(true);
            activePlatforms.Add(platform);
        }

        void Update()
        {
            if (GameManager.Instance.GameOver)
                return;

            RecycleOutOfViewPlatforms();
        }

        void InitializePlatformPool()
        {
            for (int i = 0; i < maxSegments; i++)
            {
                Platform platform = Instantiate(platformPrefab);
                platform.gameObject.SetActive(false);
                platformPool.Enqueue(platform);
            }
        }

        //void SpawnInitialPlatforms()
        //{
        //    for (int i = 0; i < maxSegments; i++)
        //    {
        //        Vector3 spawnPosition = new Vector3(playerTransform.position.x, 0, i * platformLength);
        //        SpawnPlatform(spawnPosition);
        //    }
        //}

        // Reset the world by shifting platforms back based on the player's offset
        public void ResetWorld(float offset)
        {
            foreach (Platform platform in activePlatforms)
            {
                platform.transform.position -= new Vector3(0, 0, offset);  // Shift platforms backward by the player's reset distance
            }
        }

        void RecycleOutOfViewPlatforms()
        {
            if (activePlatforms.Count > 0)
            {
                Platform firstPlatform = activePlatforms[0];
                if (firstPlatform.transform.position.z < playerTransform.position.z - platformLength)
                {
                    RecyclePlatform();
                }
            }
        }

        void RecyclePlatform()
        {
            Platform oldPlatform = activePlatforms[0];
            activePlatforms.RemoveAt(0);
            platformPool.Enqueue(oldPlatform);
            oldPlatform.gameObject.SetActive(false);

            Vector3 newPosition = activePlatforms[activePlatforms.Count - 1].transform.position + Vector3.forward * platformLength;
            SpawnPlatform(newPosition);
        }

        void SpawnPlatform(Vector3 position)
        {
            Platform platform = platformPool.Dequeue();
            platform.transform.position = position;
            platform.AddObstaclesAndCollectibles();
            platform.gameObject.SetActive(true);
            activePlatforms.Add(platform);
        }
    }
}