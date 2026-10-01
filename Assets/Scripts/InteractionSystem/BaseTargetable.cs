using Unity.Netcode;
using UnityEngine;

namespace InteractionSystem
{
    public abstract class BaseTargetable : NetworkBehaviour, ITargetable
    {
        public virtual string TargetableName => gameObject.name;

        public void OnTarget()
        {
            // TODO highlight
        }

        public void OffTarget()
        {
            // TODO end highlight
        }
    }
}