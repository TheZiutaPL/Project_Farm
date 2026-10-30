using UnityEngine;
using GameTime;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;
using GameLobby;

[RequireComponent(typeof(CanvasGroup))]
public class GameEndUI : NetworkBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        restartButton.onClick.AddListener(OnRestartButton);
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

        statsText.SetText("TEMP_STATS");
        restartButton.gameObject.SetActive(IsHost);
    }

    private void OnRestartButton()
    {
        if (!IsHost)
            return;

        Debug.Log("Performing restart");
        NetworkManager.SceneManager.LoadScene(Constants.GAME_SCENE_NAME, UnityEngine.SceneManagement.LoadSceneMode.Single);
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
