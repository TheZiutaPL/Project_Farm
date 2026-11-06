using System;
using Unity.Netcode;
using UnityEngine;

namespace InteractionSystem
{
    public abstract class BaseTargetable : NetworkBehaviour, ITargetable
    {
        public virtual string TargetableName => gameObject.name;

        public Action<bool> OnTargetAction;

        public void OnTarget()
        {
            OnTargetAction?.Invoke(true);
        }

        public void OffTarget()
        {
            OnTargetAction?.Invoke(false);
        }
    }
}