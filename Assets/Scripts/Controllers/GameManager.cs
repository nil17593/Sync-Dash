using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singletone<GameManager>
{
    public float dist;
    public float currentSpeed;
    public bool GameOver;
    protected override void Awake()
    {
        base.Awake();
    }

    public void RestartGame()
    {
        currentSpeed = 5;
        GameOver = false;
    }
}




