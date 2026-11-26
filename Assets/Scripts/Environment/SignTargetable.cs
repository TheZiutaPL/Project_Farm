using InteractionSystem;
using Popups;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SignTargetable : MonoBehaviour, ITargetable, IPopable
{
    [SerializeField] private string signText;

    public string TargetableName => signText;
    public string PopupName => signText;

    public Action<bool> OnTargetAction { get; set; }

    public PopupType PopupType => PopupType.Header;

    public Vector3 Position => transform.position + Vector3.up;

    public Dictionary<string, object> GetPopupInfo() => null;

    private PopupInstance popupInstance;

    public void OnTarget()
    {
        popupInstance = PopupUI.SetPopup(this);
    }

    public void OffTarget()
    {
        popupInstance.ReleasePopup();
    }
}
