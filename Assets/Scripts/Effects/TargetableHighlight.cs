using InteractionSystem;
using UnityEngine;

[RequireComponent(typeof(BaseTargetable))]
public class TargetableHighlight : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers = new Renderer[0];

    private const string SELECTION_BOOL_KEY = "_Selected";

    private void Awake()
    {
        GetComponent<BaseTargetable>().OnTargetAction += SetHighlight;
    }

    private void SetHighlight(bool set)
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.SetInt(SELECTION_BOOL_KEY, set ? 1 : 0);
    }
}
