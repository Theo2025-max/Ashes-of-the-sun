using UnityEngine;
using System;

public static class PlayerEvents
{
    public static event Action<Transform> OnPlayerSpawned;

    public static void PlayerSpawned(Transform playerTransform)
    {
        OnPlayerSpawned?.Invoke(playerTransform);
    }
}