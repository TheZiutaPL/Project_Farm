using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private LobbyListItemUI lobbyItemPrefab;

    private List<LobbyListItemUI> lobbyList = new List<LobbyListItemUI>();

    public async void RefreshLobbyList()
    {
        if (!LobbyManager.IsSignedIn)
            return;

        ClearLobbyList();

        QueryResponse foundLobbies = await LobbyManager.GetLobbies();

        if (foundLobbies == null)
            return;

        for (int i = 0; i < foundLobbies.Results.Count; i++)
        {
            LobbyListItemUI lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);

            lobbyItem.SetLobby(foundLobbies.Results[i]);

            lobbyList.Add(lobbyItem);
        }
    }
    
    private void ClearLobbyList()
    {
        for (int i = 0; i < lobbyList.Count; i++)
        {
            Destroy(lobbyList[i].gameObject);
        }

        lobbyList.Clear();
    }


    private void OnSignIn()
    {
        RefreshLobbyList();
    }

    private void OnEnable()
    {
        LobbyManager.OnSingedIn += OnSignIn;
    }

    private void OnDisable()
    {
        LobbyManager.OnSingedIn -= OnSignIn;
    }
}
