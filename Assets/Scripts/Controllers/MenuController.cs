using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleBucks.SyncDash
{
    /// <summary>
    /// Menu controller class which deal with user choices on main scene
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
        public void OnPlayButtonClicked()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void OnQuitButtonClicked()
        {
            Application.Quit();
        }
    }
}
