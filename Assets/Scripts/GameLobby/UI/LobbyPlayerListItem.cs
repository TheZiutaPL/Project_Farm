using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLobby.UI
{
    public class LobbyPlayerListItem : MonoBehaviour
    {
        private string playerID;

        [SerializeField] private TextMeshProUGUI playerNameText;

        [SerializeField] private Toggle readyToggle;
        [SerializeField] private GameObject hostObject;
        [SerializeField] private GameObject youObject;

        [SerializeField] private GameObject playerActionsButtons;
        [SerializeField] private Button promotePlayerButton;
        [SerializeField] private Button kickPlayerButton;

        private void Awake()
        {
            promotePlayerButton.onClick.AddListener(PromotePlayer);
            kickPlayerButton.onClick.AddListener(KickPlayer);
        }

        public void SetPlayer(string playerID, string playerName, bool isHost, bool isYou, bool isReady)
        {
            this.playerID = playerID;

            playerNameText.SetText(playerName);

            readyToggle.isOn = isReady;

            readyToggle.gameObject.SetActive(!isHost);
            hostObject.SetActive(isHost);

            youObject.SetActive(isYou);

            playerActionsButtons.SetActive(!isHost && !isYou);
        }

        private void PromotePlayer()
        {
            LobbyManager.Instance.PromoteToHost(playerID);
        }

        private void KickPlayer()
        {
            LobbyManager.Instance.KickPlayer(playerID);
        }
    }
}