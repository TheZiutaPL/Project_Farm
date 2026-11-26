using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLobby.UI
{
    public class LobbyCreateUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField lobbyNameInputField;
        [SerializeField] private Toggle privateToggle;

        // TEMP this is awful
        [SerializeField] private GameObject[] maxPlayersSelectionVisuals = new GameObject[0];
        [SerializeField] private GameObject startPlayerSelectionVisual;

        private int maxPlayerCount;

        private void Awake()
        {
            for (int i = 0; i < maxPlayersSelectionVisuals.Length; i++)
                maxPlayersSelectionVisuals[i].SetActive(false);
            startPlayerSelectionVisual.SetActive(true);
        }

        private void OnEnable()
        {
            privateToggle.isOn = false;
        }

        public void CreateLobby()
        {
            if (!LobbyManager.IsSignedIn)
                return;

            int playerCount = Mathf.Clamp(maxPlayerCount, 2, 8);
            LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, playerCount, privateToggle.isOn);
        }

        public void SetPlayerCount(int count)
        {
            maxPlayerCount = count;

            for (int i = 0; i < maxPlayersSelectionVisuals.Length; i++)
                maxPlayersSelectionVisuals[i].SetActive(false);
        }
    }
}