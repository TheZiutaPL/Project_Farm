using System;
using UnityEngine;

namespace InteractionSystem
{
    public interface ITargetable
    {
        public string TargetableName { get; }

        public Action<bool> OnTargetAction { get; set; }

        public void OnTarget();
        public void OffTarget();
    }
}