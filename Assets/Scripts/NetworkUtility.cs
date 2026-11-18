using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class NetworkUtility
{
    public static void LeaveGame()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);

        if (NetworkManager.Singleton.IsHost)
        {
            // TODO host disconnection scenario
        }
    }
}
