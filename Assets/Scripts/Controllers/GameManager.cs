
namespace BattleBucks.SyncDash
{
    /// <summary>
    /// this is GameManager class manages all game data
    /// </summary>
    public class GameManager : Singletone<GameManager>
    {
        public float dist;
        public float currentSpeed;
        public bool GameOver;
        public float syncDelay;
        protected override void Awake()
        {
            base.Awake();
        }

        public void RestartGame()
        {
            currentSpeed = 5;
            syncDelay = 0.1f;
            GameOver = false;
        }
    }
}



