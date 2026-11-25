using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    public static NetworkPlayerSpawner Instance { get; private set; }

    [SerializeField] private NetworkObject playerObjectPrefab;

    [Header("Spawn distance")]
    [SerializeField, Min(1)] private int maxPlayersSpawnedInRow = 4;
    [SerializeField] private float spawnDistance;

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
        if (!IsServer || LoadedPlayers)
            return;

        if (clientsCompleted.Count > 0)
        {
            int playersInRow = Mathf.Min(clientsCompleted.Count, maxPlayersSpawnedInRow);
            int totalRows = clientsCompleted.Count / playersInRow;

            float halfRawDistance = (playersInRow - 1) * spawnDistance;
            float halfColumnDistance = (clientsCompleted.Count - 1) /  playersInRow * spawnDistance;

            for (int i = 0; i < clientsCompleted.Count; i++)
            {
                float horizontalPlacement = Mathf.Clamp01(i % playersInRow / (float)playersInRow);
                float verticalPlacement = Mathf.Clamp01(i / playersInRow / (float)totalRows);

                Vector3 spawnPosition = transform.position + new Vector3(
                    Mathf.Lerp(-halfRawDistance, halfRawDistance, horizontalPlacement),
                    0,
                    Mathf.Lerp(halfColumnDistance, -halfColumnDistance, verticalPlacement));

                SpawnPlayer(playerObjectPrefab, spawnPosition, clientsCompleted[i]);
            }
        }

        Debug.Log("Loaded all players!");
        LoadedPlayers = true;
    }

    private void OnClientLateJoined(ulong client)
    {
        // Stops if is not the server
        if (!IsServer)
            return;

        // Stops if the player is not late
        if (!LoadedPlayers)
            return;

        SpawnPlayer(playerObjectPrefab, transform.position, client);

        Debug.Log($"Player {client} late joined!");
    }

    private NetworkObject SpawnPlayer(NetworkObject playerObjectPrefab, Vector3 position, ulong client)
    {
        NetworkObject playerObject = Instantiate(playerObjectPrefab, position, Quaternion.identity);
        playerObject.SpawnAsPlayerObject(client, true);

        return playerObject;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        int toSpawn = maxPlayersSpawnedInRow * maxPlayersSpawnedInRow;

        float halfRawDistance = (maxPlayersSpawnedInRow - 1) * spawnDistance;
        float halfColumnDistance = (toSpawn - 1) / maxPlayersSpawnedInRow * spawnDistance;

        float minSphere = .05f;
        float maxSphere = .25f;

        for (int i = 0; i < toSpawn; i++)
        {
            float horizontalPlacement = Mathf.Clamp01(i % maxPlayersSpawnedInRow / (float)maxPlayersSpawnedInRow);
            float verticalPlacement = Mathf.Clamp01(i / maxPlayersSpawnedInRow / (float)maxPlayersSpawnedInRow);

            Vector3 spawnPosition = transform.position + new Vector3(
                Mathf.Lerp(-halfRawDistance, halfRawDistance, horizontalPlacement),
                0,
                Mathf.Lerp(halfColumnDistance, -halfColumnDistance, verticalPlacement));

            float sphereSize = toSpawn == 1 ? maxSphere : i / (float)(toSpawn - 1);
            Gizmos.DrawSphere(spawnPosition, Mathf.Lerp(maxSphere, minSphere, sphereSize));
        }
    }
#endif
}
