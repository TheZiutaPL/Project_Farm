using InteractionSystem;
using UnityEngine;

namespace InteractionSystem
{
    public class DebugInteractable : MonoBehaviour, IInteractable
    {
        public string TargetableName => "Debug";

        public void Interact()
        {
            Debug.Log($"You've used {TargetableName}");
        }

        public void OffTarget()
        {
            Debug.Log($"Target off {TargetableName}");
        }

        public void OnTarget()
        {
            Debug.Log($"Target on {TargetableName}");
        }
    }
}