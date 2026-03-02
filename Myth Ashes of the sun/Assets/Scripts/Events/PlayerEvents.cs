using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event Action<Transform> OnPlayerSpawned;

    private static Transform currentPlayer;

    public static void PlayerSpawned(Transform player)
    {
        currentPlayer = player;
        OnPlayerSpawned?.Invoke(player);
    }

    public static Transform GetCurrentPlayer()
    {
        return currentPlayer;
    }
}