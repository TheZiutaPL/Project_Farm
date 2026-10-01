using InteractionSystem;
using InventorySystem;
using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerInstance : NetworkBehaviour
{
    public static NetworkPlayerInstance OwnerInstance { get; private set; }

    public static bool IsOwnerAssigned => OwnerInstance != null;
    public static Action OnOwnerInstanceAssigned;

    [field: SerializeField] public Interactor Interactor { get; private set; }
    [field: SerializeField] public Inventory Inventory { get; private set; } 

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Destroy(this);
            return;
        }

        if (IsOwnerAssigned)
        {
            Debug.LogWarning($"There are multiple owner instance objects! Despawning {gameObject.name}!");
            Destroy(this);
            return;
        }

        OwnerInstance = this;
        OnOwnerInstanceAssigned?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        if (!IsOwnerAssigned)
            return;

        OwnerInstance = null;
    }
}
