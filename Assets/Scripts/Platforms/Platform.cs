using UnityEngine;

namespace BattleBucks.SyncDash
{
    /// <summary>
    /// attached on each paltform to manage the platform behavior
    /// </summary>
    public class Platform : MonoBehaviour
    {
        [Header("Obstacles and Collectibles")]
        public GameObject[] obstacles;      // Array of all obstacles (preloaded and deactivated)
        public GameObject[] collectibles;   // Array of all collectibles (preloaded and deactivated)

        [Header("Spawn Settings")]
        public int maxObstacles = 2;        // Max number of obstacles to activate per platform
        public int maxCollectibles = 3;     // Max number of collectibles to activate per platform
        public void AddObstaclesAndCollectibles()
        {
            // Randomly activate obstacles and collectibles when the platform is recycled
            ActivateRandomObstacles();
            ActivateRandomCollectibles();
        }

        void ActivateRandomObstacles()
        {
            // Deactivate all obstacles at the start
            if (obstacles.Length <= 0)
                return;
            foreach (GameObject obstacle in obstacles)
            {
                if (obstacle != null)
                {
                    obstacle.SetActive(false);
                }
            }

            // Randomly activate a subset of obstacles
            int numObstacles = Random.Range(1, maxObstacles + 1);
            for (int i = 0; i < numObstacles; i++)
            {
                int randomIndex = Random.Range(0, obstacles.Length);
                if (obstacles[randomIndex] != null)
                {
                    obstacles[randomIndex].SetActive(true);
                }
            }
        }

        void ActivateRandomCollectibles()
        {
            if (collectibles.Length <= 0)
                return;
            // Deactivate all collectibles at the start
            foreach (GameObject collectible in collectibles)
            {
                if (collectible != null)
                {
                    collectible.SetActive(false);
                }
            }

            // Randomly activate a subset of collectibles
            int numCollectibles = Random.Range(1, maxCollectibles + 1);
            for (int i = 0; i < numCollectibles; i++)
            {
                int randomIndex = Random.Range(0, collectibles.Length);
                if (collectibles[randomIndex] != null)
                {
                    collectibles[randomIndex].SetActive(true);
                }
            }
        }

        // This method will be called when the player collides with a collectible
        public void Collect(GameObject collectible)
        {
            // Deactivate the collectible once collected
            collectible.SetActive(false);
        }

        // Cleanup method to deactivate all items before recycling
        public void Cleanup()
        {
            // Deactivate all obstacles and collectibles
            foreach (GameObject obstacle in obstacles)
            {
                obstacle.SetActive(false);
            }
            foreach (GameObject collectible in collectibles)
            {
                collectible.SetActive(false);
            }
        }
    }
}
