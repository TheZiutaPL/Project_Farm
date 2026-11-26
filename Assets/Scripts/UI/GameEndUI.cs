using UnityEngine;
using GameTime;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;
using StatTracking;

[RequireComponent(typeof(CanvasGroup))]
public class GameEndUI : NetworkBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private string winMessage = "YOU WON";
    [SerializeField] private string loseMessage = "YOU LOST";
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button leaveButton;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        restartButton.onClick.AddListener(OnRestartButton);
        leaveButton.onClick.AddListener(OnLeaveButton);
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnGameTimePassed()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // TODO alpha tween

        bool winCondition = StatTrackingManager.Instance.GetStat(GameStats.REQUESTS_COMPLETED) >= Constants.REQUIRED_REQUESTS_TO_WIN;
        endText.SetText(winCondition ? winMessage : loseMessage);

        statsText.SetText(StatTrackingManager.Instance.GetStatString());
        restartButton.gameObject.SetActive(IsHost);
    }

    private void OnRestartButton()
    {
        if (!IsHost)
            return;

        Debug.Log("Performing restart");
        NetworkManager.SceneManager.LoadScene(Constants.GAME_SCENE_NAME, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void OnLeaveButton()
    {
        NetworkDisconnectionManager.LeaveGame();
    }

    private void OnEnable()
    {
        GameTimeManager.OnGameTimeEnded += OnGameTimePassed;
    }

    private void OnDisable()
    {
        GameTimeManager.OnGameTimeEnded -= OnGameTimePassed;
    }
}
