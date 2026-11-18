using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GameLobby.UI
{
    public class LobbyHostObject : MonoBehaviour
    {
        [SerializeField] private bool clientObject;

        private void Awake()
        {
            LobbyManager.OnLobbyUpdated += OnLobbyUpdated;
            LobbyManager.OnLobbyJoined += OnLobbyUpdated;
            RefreshObject();
        }

        private void OnDestroy()
        {
            LobbyManager.OnLobbyUpdated -= OnLobbyUpdated;
            LobbyManager.OnLobbyJoined -= OnLobbyUpdated;
        }

        private void OnLobbyUpdated(Lobby lobby) => RefreshObject();

        private void RefreshObject()
        {
            if (LobbyManager.Instance == null)
                return;

            gameObject.SetActive(LobbyManager.Instance.IsLobbyHost != clientObject);
        }
    }
}