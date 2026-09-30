using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyPlayerCountText;
    [SerializeField] private TextMeshProUGUI lobbyPrivateText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TextMeshProUGUI joinedPlayersText;

    private const string LOADED_SCENE_NAME = "GameScene";

    private void Start()
    {
        LobbyManager.OnLobbyJoined += OnLobbyJoined;
        LobbyManager.OnLobbyLeft += OnLobbyLeft;
    }

    private void OnDestroy()
    {
        LobbyManager.OnLobbyJoined -= OnLobbyJoined;
        LobbyManager.OnLobbyLeft -= OnLobbyLeft;
    }

    private void OnLobbyJoined(Lobby lobby)
    {
        MainMenuUI.Instance.SetTab("Lobby");
    }
    private void OnLobbyLeft()
    {
        MainMenuUI.Instance.SetTab("Play");
    }

    private void OnEnable()
    {
        LobbyManager.OnLobbyUpdated += OnLobbyUpdate;

        if (!LobbyManager.IsSignedIn)
            return;

        OnLobbyUpdate(LobbyManager.Instance.JoinedLobby);
    }

    private void OnDisable()
    {
        LobbyManager.OnLobbyUpdated -= OnLobbyUpdate;
    }

    private void OnLobbyUpdate(Lobby lobby)
    {
        if (lobby == null)
            return;

        lobbyNameText.SetText(lobby.Name);
        lobbyPlayerCountText.SetText($"{lobby.Players.Count}/{lobby.MaxPlayers}");
        lobbyPrivateText.SetText($"Is Private: {lobby.IsPrivate}");
        lobbyCodeText.SetText(lobby.LobbyCode);

        string joinedPlayersString = string.Empty;
        for (int i = 0; i < lobby.Players.Count; i++)
        {
            joinedPlayersString += lobby.Players[i].Data["_playerName"].Value;

            if (lobby.HostId == lobby.Players[i].Id)
                joinedPlayersString += " (host)";

            joinedPlayersString += "\n";
        }
        joinedPlayersText.SetText(joinedPlayersString.Trim());
    }

    public void DisbandLobby()
    {
        if (!LobbyManager.IsSignedIn)
            return;

        LobbyManager.Instance.DeleteLobby();
    }

    public void LeaveLobby()
    {
        LobbyManager.Instance.LeaveLobby();
    }

    public void StartGame()
    {
        if (!LobbyManager.Instance.IsLobbyHost)
            return;

        Debug.Log("Starting game...");
        NetworkManager.Singleton.SceneManager.LoadScene(LOADED_SCENE_NAME, LoadSceneMode.Single);
    }
}
