using TMPro;
using UnityEngine;

public class LobbyJoinCodeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInputField;

    public void JoinByCode()
    {
        if (!LobbyManager.IsSignedIn)
            return;

        if (string.IsNullOrEmpty(codeInputField.text))
            return;

        LobbyManager.Instance.JoinLobbyByCode(codeInputField.text.Trim());
    }
}
