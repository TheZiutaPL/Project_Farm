using UnityEngine;

namespace InteractionSystem
{
    public interface ITargetable
    {
        public string TargetableName { get; }

        public void OnTarget();
        public void OffTarget();
    }
}