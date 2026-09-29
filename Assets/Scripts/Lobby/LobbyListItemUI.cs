using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyPlayersText;
    private Lobby lobby;

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;

        lobbyNameText.SetText(lobby.Name);
        lobbyPlayersText.SetText($"{lobby.Players.Count}/{lobby.MaxPlayers}");
    }

    public void JoinLobby()
    {
        if (lobby == null)
            return;

        LobbyManager.Instance.JoinLobbyByID(lobby.Id);
    }
}
