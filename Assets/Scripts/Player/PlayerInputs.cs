using Unity.Netcode;
using UnityEngine;

public class PlayerInputs : NetworkBehaviour
{
    public GameInputs Inputs { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        Inputs = new GameInputs();

        Inputs.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        Inputs.Disable();
    }
}
