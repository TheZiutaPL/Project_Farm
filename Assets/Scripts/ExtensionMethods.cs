using Unity.Netcode;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool GetNetworkPlayerInstance(this RpcParams rpcParams, out NetworkPlayerInstance networkPlayerInstance)
    {
        networkPlayerInstance = rpcParams.GetNetworkPlayerInstance();

        return networkPlayerInstance != null;
    }
    public static NetworkPlayerInstance GetNetworkPlayerInstance(this RpcParams rpcParams)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(rpcParams.Receive.SenderClientId, out NetworkClient networkClient))
            return null;

        if (networkClient.PlayerObject == null)
            return null;

        return networkClient.PlayerObject.GetComponent<NetworkPlayerInstance>();
    }
}
