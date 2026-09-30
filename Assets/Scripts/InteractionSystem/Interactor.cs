using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InteractionSystem
{
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private float interactionRange = 2;

        private Collider[] colls = new Collider[10];

        private ITargetable currentTargetable;
        private IInteractable currentInteractable;

        private void SetTargetable(ITargetable targetable)
        {
            if (currentTargetable == targetable)
                return;

            if (UtilityMethods.IsInterfaceValid(currentTargetable))
                currentTargetable.OffTarget();

            if (!UtilityMethods.IsInterfaceValid(targetable))
            {
                currentTargetable = null;
                return;
            }

            currentTargetable = targetable;

            // Get interactable
            if (targetable is IInteractable interactable)
                currentInteractable = interactable;
            else
                currentInteractable = null;

            targetable.OnTarget();
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            PerformInteractableLookupUpdate();
        }

        private void PerformInteractableLookupUpdate()
        {
            int found = Physics.OverlapSphereNonAlloc(transform.position, interactionRange, colls, interactionMask);

            // Finds first found targetable
            // TODO find closest
            ITargetable targetable = null;
            for (int i = 0; i < found; i++)
            {
                if (!colls[i].TryGetComponent(out ITargetable temp) || !UtilityMethods.IsInterfaceValid(temp))
                    continue;

                targetable = temp;
                break;
            }

            SetTargetable(targetable);
        }

        public void Interact(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            if (!UtilityMethods.IsInterfaceValid(currentInteractable))
                return;

            currentInteractable.Interact();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}