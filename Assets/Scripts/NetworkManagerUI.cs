using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(OnHostButton);
        clientButton.onClick.AddListener(OnClientButton);
    }

    private void OnHostButton()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void OnClientButton()
    {
        NetworkManager.Singleton.StartClient();
    }
}
