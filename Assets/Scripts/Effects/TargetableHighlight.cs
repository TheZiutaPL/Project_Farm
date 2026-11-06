using InteractionSystem;
using UnityEngine;

[RequireComponent(typeof(BaseTargetable))]
public class TargetableHighlight : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers = new Renderer[0];

    private const string SELECTION_BOOL_KEY = "_Selected";

    private void Awake()
    {
        GetComponent<BaseTargetable>().OnTargetAction += OnTargetAction;
    }

    private void OnTargetAction(bool target)
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.SetInt(SELECTION_BOOL_KEY, target ? 1 : 0);
    }
}
