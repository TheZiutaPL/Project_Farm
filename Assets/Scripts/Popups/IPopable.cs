using System.Collections.Generic;
using UnityEngine;

namespace Popups
{
    public interface IPopable
    {
        public string PopupName { get; }
        public PopupType PopupType { get; }

        public Vector3 Position { get; }

        public Dictionary<string, object> GetPopupInfo();
    }
}