using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkDisconnectionManager : NetworkBehaviour
{
    private static NetworkDisconnectionManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static void LeaveGame()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.Shutdown();        

        if (instance != null && NetworkManager.Singleton.IsServer)
        {
            instance.OnHostDiconnectedRpc();
        }
        else
            SceneManager.LoadScene(0);
    }

    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
    private void OnHostDiconnectedRpc()
    {
        // TODO host disconnection scenario (maybe some kind of re-hosting with other players?)

        SceneManager.LoadScene(0);
    }
}
