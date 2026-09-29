using Mono.Cecil.Cil;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    private Lobby hostLobby;
    private Lobby joinedLobby;

    public Lobby GetJoinedLobby() => joinedLobby;

    [Header("Updated")]
    [SerializeField] private float heartbeatTime = 20f;
    private float heartbeatTimer;
    [SerializeField] private float lobbyPullTime = 4f;
    private float lobbyPullTimer;

    public static Action OnSingedIn;
    public static Action<Lobby> OnLobbyCreated;
    public static Action<Lobby> OnLobbyJoined;
    public static Action OnLobbyLeft;
    public static Action<Lobby> OnLobbyUpdated;

    public static bool IsSignedIn => UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn;

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
        Data = new System.Collections.Generic.Dictionary<string, PlayerDataObject>()
        {
            { "_playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, defaultPlayerName) }
        }
    };

    private void Update()
    {
        PerformLobbyHeartbeatUpdate();

        PerformLobbyPullUpdate();
    }

    private async void PerformLobbyHeartbeatUpdate()
    {
        if (hostLobby == null)
            return;

        heartbeatTimer += Time.deltaTime;

        if(heartbeatTimer > heartbeatTime)
        {
            heartbeatTimer = 0;

            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private async void PerformLobbyPullUpdate()
    {
        if (joinedLobby == null)
            return;

        lobbyPullTimer += Time.deltaTime;

        if (lobbyPullTimer > lobbyPullTime)
        {
            lobbyPullTimer = 0;

            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                // If we got kicked/disconnected
                if(lobby.Players == null)
                {
                    joinedLobby = null;

                    OnLobbyLeft.Invoke();
                    return;
                }

                // Update lobby reference
                joinedLobby = lobby;

                // Check if we are the host
                if (lobby.HostId == AuthenticationService.Instance.PlayerId)
                {
                    hostLobby = lobby;
                    await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
                }

                OnLobbyUpdated.Invoke(lobby);
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
            });

            hostLobby = lobby;
            OnLobbyCreated?.Invoke(lobby);

            joinedLobby = lobby;
            OnLobbyJoined?.Invoke(lobby);
        }
        catch(LobbyServiceException e)
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

            joinedLobby = lobby;
            OnLobbyJoined?.Invoke(lobby);
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

            joinedLobby = lobby;
            OnLobbyJoined?.Invoke(lobby);
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

            joinedLobby = lobby;
            OnLobbyJoined?.Invoke(lobby);
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
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, options);

            hostLobby = lobby;
            joinedLobby = lobby;

            OnLobbyUpdated?.Invoke(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

            OnLobbyLeft?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer(string playerID)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerID);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        if (hostLobby != null)
            return;

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);

            OnLobbyLeft?.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
