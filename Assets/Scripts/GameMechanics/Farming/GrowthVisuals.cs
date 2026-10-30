using UnityEngine;

public class GrowthVisuals : MonoBehaviour
{
    [SerializeField] private GameObject[] growthStages = new GameObject[2];

    public void UpdateGrowthStage(float growthProgress01)
    {
        int growthStage =Mathf.FloorToInt(Mathf.Clamp(growthProgress01 * growthStages.Length - 1, 0, growthStages.Length - 1));

        for (int i = 0; i < growthStages.Length; i++)
            growthStages[i].gameObject.SetActive(i == growthStage);
    }
}
