using Unity.Netcode;
using UnityEngine;

public enum ActionMaps
{
    None,
    Player,
    PauseMenu
}

public class PlayerInputs : MonoBehaviour
{
    public static PlayerInputs Instance { get; private set; }
    public GameInputs Inputs { get; private set; }
    [SerializeField] private ActionMaps startActionMap = ActionMaps.Player;

    public ActionMaps CurrentDefaultMap => GameTime.GameTimeManager.Instance.HasGameEnded ? ActionMaps.None : startActionMap;

    public void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Inputs = new GameInputs();

        Inputs.Enable();
    }

    private void Start()
    {
        SetInputActionMap(startActionMap);
    }

    public void OnDestroy()
    {
        if(Inputs != null)
            Inputs.Disable();
    }

    public void SetInputActionMap(ActionMaps map)
    {
        foreach (var actionMap in Inputs.asset.actionMaps)
        {
            if (actionMap.name != map.ToString())
                actionMap.Disable();
            else
                actionMap.Enable();
        }
    }
}
