using System;
using Unity.Netcode;
using UnityEngine;

namespace InteractionSystem
{
    public abstract class BaseTargetable : NetworkBehaviour, ITargetable
    {
        public virtual string TargetableName => gameObject.name;

        public Action<bool> OnTargetAction { get; set; }

        public void OnTarget()
        {

        }

        public void OffTarget()
        {

        }
    }
}