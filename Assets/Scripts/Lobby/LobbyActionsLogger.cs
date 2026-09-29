using System;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyActionsLogger : MonoBehaviour
{
    private void OnEnable()
    {
        LobbyManager.OnLobbyCreated += OnLobbyCreated;
        LobbyManager.OnLobbyJoined += OnLobbyJoined;

        LobbyManager.OnLobbyLeft += OnLobbyLeft;
        LobbyManager.OnLobbyUpdated += OnLobbyUpdated;
    }

    private void OnDisable()
    {
        LobbyManager.OnLobbyCreated -= OnLobbyCreated;
        LobbyManager.OnLobbyJoined -= OnLobbyJoined;

        LobbyManager.OnLobbyLeft -= OnLobbyLeft;
        LobbyManager.OnLobbyUpdated -= OnLobbyUpdated;
    }

    private string GetLobbyInfo(Lobby lobby) => $"Name: {lobby.Name} | Max Players: {lobby.MaxPlayers} | Is Private: {lobby.IsPrivate} | Host: {lobby.HostId}";

    private void OnLobbyCreated(Lobby lobby)
    {
        Debug.Log($"Created lobby! Join using code - {lobby.LobbyCode}! ({lobby.Id})" +
            $"\n" +
            GetLobbyInfo(lobby));
    }

    private void OnLobbyJoined(Lobby lobby)
    {
        Debug.Log($"Joined lobby! ({lobby.Id})" +
            $"\n" +
            GetLobbyInfo(lobby));
    }

    private void OnLobbyLeft()
    {
        Debug.Log($"You left a lobby.");
    }

    private void OnLobbyUpdated(Lobby lobby)
    {
        Debug.Log("Lobby got updated!" +
            "\n" +
            GetLobbyInfo(lobby));
    }
}
