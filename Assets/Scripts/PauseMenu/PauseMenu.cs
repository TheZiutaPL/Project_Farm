using GameTime;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PauseMenu
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseMenu : MonoBehaviour
    {
        private static PauseMenu instance;

        private CanvasGroup canvasGroup;

        private bool isOpen;
        public static bool IsOpen => instance == null ? false : instance.isOpen;

        private void Awake()
        {
            if(instance == null)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void TogglePauseMenu(InputAction.CallbackContext ctx) => TogglePauseMenu();
        public void TogglePauseMenu()
        {
            OpenPauseMenu(!isOpen);
        }

        public void OpenPauseMenu(bool open)
        {
            if (GameTimeManager.Instance.HasGameEnded)
                return;

            if (isOpen == open)
                return;

            isOpen = open;

            canvasGroup.alpha = open ? 1 : 0;
            canvasGroup.interactable = open;
            canvasGroup.blocksRaycasts = open;

            PlayerInputs.Instance.SetInputActionMap(open ? ActionMaps.PauseMenu : PlayerInputs.Instance.CurrentDefaultMap);
        }

        public void LeaveGame() => NetworkDisconnectionManager.LeaveGame();

        private void OnEnable()
        {
            PlayerInputs.Instance.Inputs.Player.TogglePauseMenu.performed += TogglePauseMenu;
            PlayerInputs.Instance.Inputs.PauseMenu.TogglePauseMenu.performed += TogglePauseMenu;
        }

        private void OnDisable()
        {
            PlayerInputs.Instance.Inputs.Player.TogglePauseMenu.performed -= TogglePauseMenu;
            PlayerInputs.Instance.Inputs.PauseMenu.TogglePauseMenu.performed -= TogglePauseMenu;
        }
    }
}