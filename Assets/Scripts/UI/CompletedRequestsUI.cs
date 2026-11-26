using TMPro;
using UnityEngine;
using GameTime;
using StatTracking;

public class CompletedRequestsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI progressText;

    private void OnEnable()
    {
        StatTrackingManager.OnStatUpdated += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        StatTrackingManager.OnStatUpdated -= Refresh;
    }

    private void Refresh(GameStats stat, int value)
    {
        if (stat != GameStats.REQUESTS_COMPLETED)
            return;

        Refresh();
    }

    private void Refresh()
    {
        float gameHours = GameTimeManager.Instance.GetGameHours();

        int hours = Mathf.FloorToInt(gameHours);
        int minutes = Mathf.RoundToInt((gameHours % 1) * 60f);

        string str = $"Complete {Constants.REQUIRED_REQUESTS_TO_WIN} requests in {hours} hours";

        if (minutes > 0)
            str += $" and {minutes} minutes";

        str += ".";

        objectiveText.SetText(str);

        if(StatTrackingManager.Instance != null)
            progressText.SetText($"{StatTrackingManager.Instance.GetStat(GameStats.REQUESTS_COMPLETED)}/{Constants.REQUIRED_REQUESTS_TO_WIN}");
    }
}
