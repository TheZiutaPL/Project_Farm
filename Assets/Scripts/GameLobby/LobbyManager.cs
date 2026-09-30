using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace GameLobby
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        private Lobby joinedLobby;
        public Lobby JoinedLobby
        {

            get => joinedLobby;

            set
            {
                joinedLobby = value;

                if (joinedLobby != null)
                    SubscribeToLobbyEvents(joinedLobby);
                else
                    UnsubscribeFromLobbyEvents();
            }
        }

        [Header("Updated")]
        [SerializeField] private float heartbeatTime = 20f;
        private float heartbeatTimer;

        public static Action OnSingedIn;

        public static Action<Lobby> OnLobbyUpdated;
        public static Action<Lobby> OnLobbyJoined;

        public static Action OnLobbyLeft;
        public static Action OnLobbyKicked;

        private ILobbyEvents lobbyEvents;

        public static bool IsSignedIn => UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn;
        public bool IsInLobby => JoinedLobby != null && JoinedLobby.Players != null;
        public bool IsLobbyHost => IsInLobby && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += OnSignIn;

            // For now the player is signed anonymously
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        [SerializeField] private string defaultPlayerName = "Disarmed Cookie";
        private Player GetPlayerData() => new Player(AuthenticationService.Instance.PlayerId)
        {
            Data = new Dictionary<string, PlayerDataObject>()
        {
            { "_playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, defaultPlayerName) }
        }
        };

        private void Update()
        {
            PerformLobbyHeartbeatUpdate();
        }

        private async void PerformLobbyHeartbeatUpdate()
        {
            if (!IsLobbyHost)
                return;

            heartbeatTimer += Time.deltaTime;

            if (heartbeatTimer > heartbeatTime)
            {
                heartbeatTimer = 0;

                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        private void OnSignIn()
        {
            Debug.Log($"Signed in - {AuthenticationService.Instance.PlayerId}");

            OnSingedIn?.Invoke();
        }

        public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, new CreateLobbyOptions()
                {
                    IsPrivate = isPrivate,
                    Player = GetPlayerData(),

                    Data = new Dictionary<string, DataObject>()
                {
                    { LOBBY_RELAY_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, string.Empty) }
                }
                });

                JoinedLobby = lobby;
                OnLobbyJoined?.Invoke(lobby);

                await StartRelayConnectionInLobby();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public static async Task<QueryResponse> GetLobbies(QueryLobbiesOptions options = null)
        {
            try
            {
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);

                return queryResponse;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            return null;
        }

        public async void JoinLobbyByID(string lobbyID)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, new JoinLobbyByIdOptions()
                {
                    Player = GetPlayerData(),
                });

                JoinedLobby = lobby;
                OnLobbyJoined?.Invoke(lobby);

                await JoinRelayConnectionInLobby();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        public async void JoinLobbyByCode(string code)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, new JoinLobbyByCodeOptions
                {
                    Player = GetPlayerData(),
                });

                JoinedLobby = lobby;
                OnLobbyJoined?.Invoke(lobby);

                await JoinRelayConnectionInLobby();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void QuickJoin()
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(new QuickJoinLobbyOptions()
                {
                    Player = GetPlayerData(),
                });

                JoinedLobby = lobby;
                OnLobbyJoined?.Invoke(lobby);

                await JoinRelayConnectionInLobby();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void UpdateLobby(UpdateLobbyOptions options)
        {
            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public void LeaveLobby() => KickPlayer(AuthenticationService.Instance.PlayerId);
        public async void KickPlayer(string playerID)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, playerID);

                if (playerID == AuthenticationService.Instance.PlayerId)
                    LeaveLobbyCallback();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void DeleteLobby()
        {
            if (!IsLobbyHost)
                return;

            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(JoinedLobby.Id);

                LeaveLobbyCallback();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private void LeaveLobbyCallback()
        {
            JoinedLobby = null;
            NetworkManager.Singleton.Shutdown();
            OnLobbyLeft?.Invoke();
        }

        public const string LOBBY_RELAY_CODE_KEY = "_relayCode";
        private async Task StartRelayConnectionInLobby()
        {
            try
            {
                Debug.Log("Starting connection");

                string relayCode = await RelayManager.CreateRelay();

                UpdateLobby(new UpdateLobbyOptions()
                {
                    Data = new Dictionary<string, DataObject>()
                {
                    { LOBBY_RELAY_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private async Task JoinRelayConnectionInLobby()
        {
            if (JoinedLobby == null)
                return;

            try
            {
                string relayCode = JoinedLobby.Data[LOBBY_RELAY_CODE_KEY].Value;

                if (string.IsNullOrEmpty(relayCode))
                    return;

                await RelayManager.JoinRelay(relayCode);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void SubscribeToLobbyEvents(Lobby lobby)
        {
            if (lobbyEvents != null)
                UnsubscribeFromLobbyEvents();

            try
            {
                lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, GetLobbyEventCallbacks());
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void UnsubscribeFromLobbyEvents()
        {
            if (lobbyEvents == null)
                return;

            await lobbyEvents.UnsubscribeAsync();
            lobbyEvents = null;
        }

        private LobbyEventCallbacks GetLobbyEventCallbacks()
        {
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();

            callbacks.LobbyChanged += OnLobbyChanged;
            async void OnLobbyChanged(ILobbyChanges changes)
            {
                try
                {
                    changes.ApplyToLobby(JoinedLobby);
                    OnLobbyUpdated?.Invoke(JoinedLobby);

                    // If changed host
                    if (changes.HostId.Changed && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId)
                    {
                        // Start new relay
                        string relayCode = await RelayManager.CreateRelay();

                        // Update lobby's relay code
                        UpdateLobby(new UpdateLobbyOptions()
                        {
                            Data = new Dictionary<string, DataObject>()
                        {
                            { LOBBY_RELAY_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                        }
                        });
                    }

                    // If is not host and detected relay code change, join the new relay
                    if (JoinedLobby.HostId != AuthenticationService.Instance.PlayerId && changes.Data.Changed && changes.Data.Value.TryGetValue(LOBBY_RELAY_CODE_KEY, out ChangedOrRemovedLobbyValue<DataObject> relayCodeData))
                    {
                        await RelayManager.JoinRelay(relayCodeData.Value.Value);
                    }
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }

            callbacks.KickedFromLobby += OnKickedFromLobby;
            void OnKickedFromLobby()
            {
                OnLobbyKicked?.Invoke();
            }

            return callbacks;
        }

        private const string LOADED_SCENE_NAME = "GameScene";
        public void StartGame()
        {
            if (!IsLobbyHost)
                return;

            Debug.Log("Starting game...");
            NetworkManager.Singleton.SceneManager.LoadScene(LOADED_SCENE_NAME, LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            UnsubscribeFromLobbyEvents();
        }
    }
}