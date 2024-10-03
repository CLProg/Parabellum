using UnityEngine;

public static class GameEvents
{
    public static event System.Action OnMobKilled;

    public static void MobKilled()
    {
        OnMobKilled?.Invoke();
    }
}