using Unity.Netcode;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public static PlayerInputs Instance { get; private set; }
    public GameInputs Inputs { get; private set; }

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

    public void OnDestroy()
    {
        if(Inputs != null)
            Inputs.Disable();
    }
}
