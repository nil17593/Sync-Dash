using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is the event Manager class gets different created events and triggers them when needed
/// </summary>
public static class EventManager
{
    public static event Action<float> OnWorldReset;
    public static event Action OnGameOver;
    // Trigger the PlayerKilled event
    public static void TriggerWorldResetEvent(float offset)
    {
        OnWorldReset?.Invoke(offset);
    }

    public static void TriggerGameOverEvent()
    {
        OnGameOver?.Invoke();
    }
}



