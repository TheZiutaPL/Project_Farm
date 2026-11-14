using GameTime;
using TMPro;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private Transform clockWheelTransform;
    [SerializeField] private float midnightRotation = -180;
    [SerializeField] private TextMeshProUGUI clockText;

    private void Update()
    {
        if (GameTimeManager.Instance == null)
            return;

        float progress = GameTimeManager.Instance.GetInGameTimeProgress01();

        clockWheelTransform.rotation = Quaternion.Euler(0, 0, midnightRotation + progress * 360f);

        (int hour, int minute) = GameTimeManager.Instance.GetInGameTime();
        clockText.SetText($"{hour:00}:{minute:00}");
    }
}
