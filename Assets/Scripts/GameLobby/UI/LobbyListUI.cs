using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GameLobby.UI
{
    public class LobbyListUI : MonoBehaviour
    {
        [SerializeField] private Transform lobbyItemParent;
        [SerializeField] private LobbyListItemUI lobbyItemPrefab;
        [SerializeField] private TMP_InputField searchBar;

        private List<LobbyListItemUI> lobbyList = new List<LobbyListItemUI>();

        private void Awake()
        {
            if (searchBar != null)
                searchBar.onSubmit.AddListener(OnSearchBarStringChanged);
        }

        public async void RefreshLobbyList()
        {
            if (!LobbyManager.IsSignedIn)
                return;

            ClearLobbyList();

            QueryResponse foundLobbies = await LobbyManager.GetLobbies(GetQueryLobbiesOptions());

            if (foundLobbies == null)
                return;

            for (int i = 0; i < foundLobbies.Results.Count; i++)
            {
                LobbyListItemUI lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);

                lobbyItem.SetLobby(foundLobbies.Results[i]);

                lobbyList.Add(lobbyItem);
            }
        }

        private QueryLobbiesOptions GetQueryLobbiesOptions()
        {
            if (searchBar == null || string.IsNullOrWhiteSpace(searchBar.text))
                return null;

            return new QueryLobbiesOptions()
            {
                Filters = new List<QueryFilter>()
                    {
                        new QueryFilter(QueryFilter.FieldOptions.Name, searchBar.text, QueryFilter.OpOptions.CONTAINS)
                    }
            };
        }

        private void ClearLobbyList()
        {
            for (int i = 0; i < lobbyList.Count; i++)
            {
                Destroy(lobbyList[i].gameObject);
            }

            lobbyList.Clear();
        }

        private void OnSearchBarStringChanged(string searchBarString)
        {
            if (!gameObject.activeInHierarchy)
                return;

            RefreshLobbyList();
        }

        private void OnSignIn()
        {
            RefreshLobbyList();
        }

        private void OnEnable()
        {
            LobbyManager.OnSingedIn += OnSignIn;
            RefreshLobbyList();
        }

        private void OnDisable()
        {
            LobbyManager.OnSingedIn -= OnSignIn;
        }
    }
}