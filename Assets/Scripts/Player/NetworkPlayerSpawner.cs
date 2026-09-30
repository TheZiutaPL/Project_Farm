using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    public static NetworkPlayerSpawner Instance { get; private set; }

    [SerializeField] private NetworkObject playerObjectPrefab;

    [SerializeField] private bool spawnPlayersSeparately;
    public bool LoadedPlayers { get; private set; } = false;

    public override void OnNetworkSpawn()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnPlayersLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientLateJoined;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnPlayersLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientLateJoined;
    }

    private void OnPlayersLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer || spawnPlayersSeparately)
            return;

        foreach (var client in clientsCompleted)
            SpawnPlayer(playerObjectPrefab, client);

        Debug.Log("Loaded all players!");
        LoadedPlayers = true;
    }

    private void OnClientLateJoined(ulong client)
    {
        // Stops if is not the server
        if (!IsServer)
            return;

        // Stops if the player is not late
        if (!spawnPlayersSeparately && !LoadedPlayers)
            return;

        SpawnPlayer(playerObjectPrefab, client);

        Debug.Log($"Player {client} late joined!");
    }

    private NetworkObject SpawnPlayer(NetworkObject playerObjectPrefab, ulong client)
    {
        NetworkObject playerObject = Instantiate(playerObjectPrefab);
        playerObject.SpawnAsPlayerObject(client, true);

        return playerObject;
    }
}
