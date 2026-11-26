using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace GameLobby.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyPlayerCountText;
        [SerializeField] private TextMeshProUGUI lobbyPrivateText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;

        [Header("Players")]
        [SerializeField] private LobbyPlayerListItem lobbyPlayerPrefab;
        [SerializeField] private Transform lobbyPlayerParent;

        private List<LobbyPlayerListItem> lobbyPlayerObjects = new List<LobbyPlayerListItem>();

        [Header("Ready")]
        [SerializeField] private Toggle readyToggle;
        [SerializeField] private Button startGameButton;

        private void Start()
        {
            LobbyManager.OnLobbyJoined += OnLobbyJoined;
            LobbyManager.OnLobbyLeft += OnLobbyLeft;
            LobbyManager.OnLobbyKicked += OnLobbyLeft;

            startGameButton.onClick.AddListener(StartGame);
            readyToggle.onValueChanged.AddListener(SetReady);
        }

        private void OnDestroy()
        {
            LobbyManager.OnLobbyJoined -= OnLobbyJoined;
            LobbyManager.OnLobbyLeft -= OnLobbyLeft;
            LobbyManager.OnLobbyKicked -= OnLobbyLeft;

            startGameButton.onClick.RemoveListener(StartGame);
            readyToggle.onValueChanged.RemoveListener(SetReady);
        }

        private void OnLobbyJoined(Lobby lobby)
        {
            readyToggle.SetIsOnWithoutNotify(false);
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

            bool canStartGame = true;
            foreach (Player player in lobby.Players)
            {
                if (player.Id == lobby.HostId)
                    continue;

                bool isPlayerReady = player.Data.TryGetValue(LobbyManager.PLAYER_READY_DATA_KEY, out PlayerDataObject readyObj) &&
                    bool.TryParse(readyObj.Value, out bool ready) && ready;

                if (player.Id == AuthenticationService.Instance.PlayerId)
                    readyToggle.SetIsOnWithoutNotify(isPlayerReady);

                if (isPlayerReady)
                    continue;

                canStartGame = false;
                break;
            }

            startGameButton.interactable = canStartGame;

            RefreshPlayerList(lobby);
        }

        private void RefreshPlayerList(Lobby lobby)
        {
            // Create new or hide not needed player objects
            int lobbyItemsToCreate = lobby.Players.Count - lobbyPlayerObjects.Count;
            if(lobbyItemsToCreate > 0)
                for (int i = 0; i < lobbyItemsToCreate; i++)
                    lobbyPlayerObjects.Add(Instantiate(lobbyPlayerPrefab, lobbyPlayerParent));
            else if(lobbyItemsToCreate < 0)
            {
                lobbyItemsToCreate = Mathf.Abs(lobbyItemsToCreate);
                for (int i = 1; i <= lobbyItemsToCreate; i++)
                    lobbyPlayerObjects[^i].gameObject.SetActive(false);
            }

            // Set player objects
            for (int i = 0; i < lobby.Players.Count; i++)
            {
                Player player = lobby.Players[i];

                lobbyPlayerObjects[i].SetPlayer(
                    player.Id,
                    player.Data[LobbyManager.PLAYER_NAME_DATA_KEY].Value,
                    player.Id == lobby.HostId,
                    player.Id == AuthenticationService.Instance.PlayerId,
                    player.Data.TryGetValue(LobbyManager.PLAYER_READY_DATA_KEY, out PlayerDataObject dataObject) && dataObject != null && bool.TryParse(dataObject.Value, out bool result) && result);

                lobbyPlayerObjects[i].gameObject.SetActive(true);
            }
        }

        public void DisbandLobby()
        {
            if (!LobbyManager.IsSignedIn)
                return;

            LobbyManager.Instance.DeleteLobby();
        }

        public void LeaveLobby()
        {
            if (!LobbyManager.IsSignedIn)
                return;

            LobbyManager.Instance.LeaveLobby();
        }

        public void SetReady(bool ready)
        {
            if (!LobbyManager.IsSignedIn)
                return;

            LobbyManager.Instance.UpdatePlayerData((LobbyManager.PLAYER_READY_DATA_KEY, ready.ToString()));
        }

        public void StartGame()
        {
            if (!LobbyManager.IsSignedIn)
                return;

            LobbyManager.Instance.StartGame();
        }
    }
}