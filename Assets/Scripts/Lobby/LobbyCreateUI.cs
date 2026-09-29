using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Toggle privateToggle;

    public void CreateLobby()
    {
        if (!LobbyManager.IsSignedIn)
            return;

        LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, 2, privateToggle.isOn);
    }
}
