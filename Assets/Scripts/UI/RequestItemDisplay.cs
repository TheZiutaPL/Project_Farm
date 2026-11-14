using InventorySystem.UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class RequestItemDisplay : ItemDisplay
{
    private Toggle completionToggle;

    private void Awake()
    {
        completionToggle = GetComponent<Toggle>();
    }

    public void SetCompleted(bool completed)
    {
        completionToggle.isOn = completed;
    }
}
