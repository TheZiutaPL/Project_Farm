using UnityEngine;

namespace InteractionSystem
{
    public interface IInteractable : ITargetable
    {
        public void Interact();
    }
}